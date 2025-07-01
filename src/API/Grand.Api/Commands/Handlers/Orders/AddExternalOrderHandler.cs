using Grand.Api.Commands.Models.Orders;
using Grand.Api.DTOs.Orders;
using Grand.Business.Core.Extensions;
using Grand.Business.Core.Interfaces.Catalog.Prices;
using Grand.Business.Core.Interfaces.Catalog.Products;
using Grand.Business.Core.Interfaces.Checkout.Orders;
using Grand.Business.Core.Interfaces.Checkout.Shipping;
using Grand.Business.Core.Interfaces.Common.Directory;
using Grand.Business.Core.Interfaces.Common.Localization;
using Grand.Business.Core.Interfaces.Common.Logging;
using Grand.Business.Core.Interfaces.Common.Stores;
using Grand.Business.Core.Interfaces.Customers;
using Grand.Business.Core.Utilities.Checkout;
using Grand.Domain.Common;
using Grand.Domain.Customers;
using Grand.Domain.Directory;
using Grand.Domain.Orders;
using Grand.Domain.Payments;
using Grand.Domain.Shipping;
using Grand.Domain.Stores;
using MediatR;
using MongoDB.Driver;
using StackExchange.Redis;
using Order = Grand.Domain.Orders.Order;


namespace Grand.Api.Commands.Handlers.Orders
{
    public class AddExternalOrderHandler : IRequestHandler<AddExternalOrderCommand, CreateOrderResult>
    {

        private readonly ILogger _logger;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IUserFieldService _userFieldsService;
        private readonly ICurrencyService _currencyService;
        private readonly ICountryService _countryService;
        //private readonly IMongoClient _mongoClient;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStoreService _storeService;
        private readonly IPricingService _pricingService;
        private readonly ILanguageService _languageService;
        private readonly IShipmentService _shipmentService;
        public AddExternalOrderHandler(ICustomerService customerService, ILogger logger, IProductService productService, IOrderService orderService,
            IUserFieldService userFieldsService, ICurrencyService currencyService, IMediator mediator, ICountryService countryService,
            /*IMongoClient mongoClient,*/ IStoreService storeService, ILanguageService languageService, IPricingService pricingService, IShoppingCartService shoppingCartService, IShipmentService shipmentService)
        {
            _customerService = customerService;
            _logger = logger;
            _productService = productService;
            _orderService = orderService;
            _userFieldsService = userFieldsService;
            _currencyService = currencyService;
            _countryService = countryService;
            //_mongoClient = mongoClient;
            _storeService = storeService;
            _languageService = languageService;
            _pricingService = pricingService;
            _shoppingCartService = shoppingCartService;
            _shipmentService = shipmentService;
        }

        public async Task<CreateOrderResult> Handle(AddExternalOrderCommand request, CancellationToken cancellationToken)
        {
            //using (var session = await _mongoClient.StartSessionAsync(cancellationToken: cancellationToken))
            //{
            //    session.StartTransaction();
            try
            {
                Currency currency = null;

                var language = (await _languageService.GetAllLanguages()).FirstOrDefault();

                var store = (await _storeService.GetAllStores()).FirstOrDefault();
                if (store == null)
                {
                    await _logger.Warning("Store Not Found");

                    return new CreateOrderResult {
                        IsSuccess = false,
                        Message =
                            "Store Not Found"
                    };
                }

                string orderId = "";
                foreach (var content in request.Model.Content)
                {

                    var allSkusExist = content.Lines.All(item =>
                        _productService.GetProductBySku(item.Sku).Result != null);
                    if (!allSkusExist)
                    {
                        await _logger.Warning(
                            "Some SKUs in the order are not defined in the system. The order was not processed.");

                        return new CreateOrderResult {
                            IsSuccess = false,
                            Message =
                                "Some SKUs in the order are not defined in the system. The order was not processed."
                        };
                    }

                    var customer = await _customerService.InsertGuestCustomer(new Store());
                    if (customer != null)
                    {

                        await _userFieldsService.SaveField(customer, SystemCustomerFieldNames.FirstName,
                            content.CustomerFirstName);
                        await _userFieldsService.SaveField(customer, SystemCustomerFieldNames.LastName,
                            content.CustomerLastName);
                        await _userFieldsService.SaveField(customer,
                            SystemCustomerFieldNames.ImpersonatedCustomerId, content.CustomerId);
                        await _userFieldsService.SaveField(customer, SystemCustomerFieldNames.StreetAddress,
                            content.InvoiceAddress?.Address1);
                        await _userFieldsService.SaveField(customer, SystemCustomerFieldNames.StreetAddress2,
                            content.InvoiceAddress?.Address2);
                    }

                    if (customer != null)
                    {
                        customer.StoreId = store.Id;
                        customer.Email = content.CustomerEmail;
                        customer.Username = content.CustomerEmail;
                        await _customerService.UpdateCustomer(customer);

                        if (!string.IsNullOrEmpty(content.CurrencyCode))
                        {
                            currency = await _currencyService.GetCurrencyByCode(content.CurrencyCode)
                                       ?? await _currencyService.GetPrimaryStoreCurrency();


                            await _userFieldsService.SaveField(customer, SystemCustomerFieldNames.CurrencyId,
                                currency.Id);
                        }
                        else
                        {
                            currency = await _currencyService.GetPrimaryStoreCurrency();
                        }

                        var billingAddress = await CreateAddressAsync(customer, content.InvoiceAddress, AddressType.Billing);
                        var shippingAdress = await CreateAddressAsync(customer, content.ShipmentAddress, AddressType.Shipping);

                        ICollection<OrderItem> orderItems = new List<OrderItem>();

                        foreach (var item in content.Lines)
                        {
                            var product = await _productService.GetProductBySku(item.Sku);

                            if (product != null)
                                await _shoppingCartService.AddToCart(customer, product.Id,
                                    ShoppingCartType.ShoppingCart, Guid.NewGuid().ToString(),
                                    quantity: item.Quantity);

                            var orderItem = new OrderItem {
                                OrderItemGuid = Guid.NewGuid(),
                                ProductId = product.Id,
                                VendorId = product.VendorId,
                                WarehouseId = product.WarehouseId,

                                Sku = product.Sku,
                                UnitPriceInclTax = item.Price,
                                UnitPriceExclTax = item.Price - (item.VatBaseAmount ?? 0),

                                PriceInclTax = Math.Round(item.Price * item.Quantity, 2),
                                PriceExclTax = Math.Round((item.Price - (item.VatBaseAmount ?? 0)) * item.Quantity, 2),

                                TaxRate = (item.VatBaseAmount > 0 && item.Price > 0)
                                    ? ((item.VatBaseAmount ?? 0) / item.Price * 100)
                                    : 0,
                                OriginalProductCost = await _pricingService.GetProductCost(product, null), // custom attributes yoksa ""

                                AttributeDescription = item.ProductSize.Trim(),
                                Quantity = item.Quantity,
                                OpenQty = item.Quantity,
                                DiscountAmountInclTax = item.Discount,
                                DiscountAmountExclTax = item.Discount,
                                DownloadCount = 0,
                                IsDownloadActivated = false,
                                LicenseDownloadId = "",
                                IsShipEnabled = product.IsShipEnabled,
                                CreatedOnUtc = DateTime.UtcNow,
                                
                            };
                            orderItems.Add(orderItem);
                        }


                        long orderDateUnixMs = content.OrderDate;
                        DateTime orderDate = DateTimeOffset.FromUnixTimeMilliseconds(orderDateUnixMs).UtcDateTime;
                        var order = new Order {
                            OrderGuid = Guid.NewGuid(),
                            CustomerId = customer.Id,
                            OrderStatusId = (int)OrderStatusSystem.Pending,
                            PaymentStatusId = PaymentStatus.Pending,
                            ShippingStatusId = ShippingStatus.ShippingNotRequired,
                            CustomerCurrencyCode = currency.CurrencyCode,
                            OrderSubtotalInclTax = content.Lines.Sum(l => l.Price * l.Quantity),
                            OrderTotal = content.TotalPrice,
                            CreatedOnUtc = DateTime.UtcNow,
                            StoreId = store.Id,
                            OrderNumber = content.OrderNumber,
                            OrderItems = orderItems,
                            CustomerEmail = customer.Email,
                            CustomerLanguageId = language.Id,
                            SeId = customer.SeId,
                            PaidAmount = content.TotalPrice,
                            OrderDiscount = content.TotalDiscount,
                            FirstName = content.CustomerFirstName,
                            LastName = content.CustomerLastName,
                            ShippingAddress =shippingAdress,
                            BillingAddress = billingAddress,
                            ShippingMethod = content.ShipmentPackageStatus,
                            PaidDateUtc = orderDate,
                        };


                        if (Enum.TryParse<ShippingStatus>(content.Status, out var status))
                        {
                            order.ShippingStatusId = status;
                        }
                        else
                        {
                            order.ShippingStatusId = ShippingStatus.ShippingNotRequired;
                        }
                        var shipment = new Shipment {
                            OrderId = order.Id,
                            StoreId = store.Id, 
                            TrackingNumber = content.CargoTrackingNumber.ToString(),
                            CreatedOnUtc = DateTime.UtcNow
                        };
                        await _shipmentService.InsertShipment(shipment);
                     
                        await _orderService.InsertOrder(order);


                        await _userFieldsService.SaveField(customer, "Trendyol.ShipmentPackageId", content.Id);
                        await _userFieldsService.SaveField(customer, "Trendyol.CargoTrackingNumber",
                            content.CargoTrackingNumber);
                        await _userFieldsService.SaveField(customer, "Trendyol.CargoTrackingLink",
                            content.CargoTrackingLink);
                        await _userFieldsService.SaveField(customer, "Trendyol.CargoSenderNumber",
                            content.CargoSenderNumber);
                        await _userFieldsService.SaveField(customer, "Trendyol.CargoProviderName",
                            content.CargoProviderName);

                        orderId = order.Id;
                        await _orderService.InsertOrderNote(new OrderNote {
                            Note =
                                $"External Order Imported - PackageId: {content.Id}, Tracking: {content.CargoTrackingNumber}",
                            DisplayToCustomer = false,
                            CreatedOnUtc = DateTime.UtcNow,
                            OrderId = order.Id,
                        });

                        await _orderService.UpdateOrder(order);
                    }
                }
                
                //await session.CommitTransactionAsync(cancellationToken: cancellationToken);
                return new CreateOrderResult { IsSuccess = true,OrderId = orderId, Message = "Sipariş başarıyla oluşturuldu" };


            }
            catch (Exception ex)
            {
                //await session.AbortTransactionAsync(cancellationToken: cancellationToken);
                _logger.Error("Sipariş işleme hatası", ex);
                return new CreateOrderResult { IsSuccess = false, Message = ex.Message };
                throw;
            }
            //}
        }

        private async Task<Address> CreateAddressAsync(Customer customer, OrderAddressDto orderAddressDto, AddressType adressType)
        {
            var address = new Address {
                FirstName = orderAddressDto.FirstName,
                LastName = orderAddressDto.LastName,
                Email = customer.Email,
                Company = string.Empty,
                CountryId = await GetCountryIdAsync("Turkey"),
                StateProvinceId = await GetStateProvinceIdAsync(orderAddressDto.StateName),
                City = orderAddressDto.City,
                Address1 = orderAddressDto.Address1,
                Address2 = $"{orderAddressDto.Address2}, {orderAddressDto.District}",
                AddressType = adressType,
                ZipPostalCode = orderAddressDto.PostalCode,
                //PhoneNumber = orderAddressDto.PhoneNumber,
                CreatedOnUtc = DateTime.UtcNow
            };

            await _customerService.InsertAddress(address, customer.Id);
            return address;
        }

        private async Task<string> GetCountryIdAsync(string countryName)
        {
            var countries = await _countryService.GetAllCountries();
            return countries.FirstOrDefault(c => c.Name.Equals(countryName, StringComparison.OrdinalIgnoreCase))?.Id;
        }

        private async Task<string> GetStateProvinceIdAsync(string stateProvinceName)
        {
            var states = await _countryService.GetStateProvincesByCountryId(await GetCountryIdAsync("Turkey"), "tr-TR");
            return states.FirstOrDefault(s => s.Name.Equals(stateProvinceName, StringComparison.OrdinalIgnoreCase))?.Id;
        }
    }
}
