namespace Grand.Api.DTOs.Orders
{
    public class OrderRequestDto
    {
        public int Page { get; set; }
        public int Size { get; set; }
        public int TotalPages { get; set; }
        public int TotalElements { get; set; }
        public List<ExternalOrderDto> Content { get; set; }
    }

    public class ExternalOrderDto
    {
        public long Id { get; set; }
        public int OrderNumber { get; set; }
        public double GrossAmount { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalTyDiscount { get; set; }
        public double TotalPrice { get; set; }
        public string Status { get; set; }
        public string ShipmentPackageStatus { get; set; }
        public long OrderDate { get; set; }
        public long OriginShipmentDate { get; set; }
        public long LastModifiedDate { get; set; }

        public bool Commercial { get; set; }
        public bool Micro { get; set; }
        public bool GiftBoxRequested { get; set; }
        public bool DeliveredByService { get; set; }
        public bool ContainsDangerousProduct { get; set; }

        public string CurrencyCode { get; set; }

        public string TcIdentityNumber { get; set; }
        public string IdentityNumber { get; set; }

        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerEmail { get; set; }
        public long CustomerId { get; set; }

        public string CargoProviderName { get; set; }
        public string CargoSenderNumber { get; set; }
        public long CargoTrackingNumber { get; set; }
        public string CargoTrackingLink { get; set; }

        public string FastDeliveryType { get; set; }
        public bool FastDelivery { get; set; }

        public string DeliveryType { get; set; }

        public long EstimatedDeliveryStartDate { get; set; }
        public long EstimatedDeliveryEndDate { get; set; }

        public long? AgreedDeliveryDate { get; set; }
        public bool? AgreedDeliveryDateExtendible { get; set; }
        public long? ExtendedAgreedDeliveryDate { get; set; }
        public long? AgreedDeliveryExtensionStartDate { get; set; }
        public long? AgreedDeliveryExtensionEndDate { get; set; }

        public string InvoiceLink { get; set; }

        public string EtgbNo { get; set; }
        public long? EtgbDate { get; set; }

        public bool _3pByTrendyol { get; set; }

        public OrderAddressDto ShipmentAddress { get; set; }
        public InvoiceAddressDto InvoiceAddress { get; set; }

        public List<ExternalOrderLineDto> Lines { get; set; }
        public List<PackageHistoryDto> PackageHistories { get; set; }
    }

    public class OrderAddressDto
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }

        public string Company { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }

        public AddressLinesDto AddressLines { get; set; }

        public string City { get; set; }
        public int CityCode { get; set; }
        public string District { get; set; }
        public int DistrictId { get; set; }
        public int CountyId { get; set; }
        public string CountyName { get; set; }
        public string ShortAddress { get; set; }
        public string StateName { get; set; }

        public string PostalCode { get; set; }
        public string CountryCode { get; set; }

        public int NeighborhoodId { get; set; }
        public string Neighborhood { get; set; }

        public string Phone { get; set; }

        public string FullAddress { get; set; }
    }

    public class InvoiceAddressDto : OrderAddressDto
    {
        public string TaxOffice { get; set; }
        public string TaxNumber { get; set; }
    }

    public class AddressLinesDto
    {
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
    }

    public class ExternalOrderLineDto
    {
        public long Id { get; set; }
        public int Quantity { get; set; }
        public int SalesCampaignId { get; set; }
        public string ProductSize { get; set; }
        public string MerchantSku { get; set; }
        public string Sku { get; set; }

        public string ProductName { get; set; }
        public long ProductCode { get; set; }
        public string ProductOrigin { get; set; }
        public int MerchantId { get; set; }

        public double Amount { get; set; }
        public double Discount { get; set; }
        public double TyDiscount { get; set; }

        public string ProductColor { get; set; }
        public string OrderLineItemStatusName { get; set; }
        public string Barcode { get; set; }
        public double? VatBaseAmount { get; set; }
        public double Price { get; set; }
        public int ProductCategoryId { get; set; }
        public double LaborCost { get; set; }

        public List<DiscountDetailDto> DiscountDetails { get; set; }
        public List<FastDeliveryOptionDto> FastDeliveryOptions { get; set; }
        public string CurrencyCode { get; set; }
    }

    public class DiscountDetailDto
    {
        public double LineItemPrice { get; set; }
        public double LineItemDiscount { get; set; }
        public double LineItemTyDiscount { get; set; }
    }

    public class FastDeliveryOptionDto
    {
        public string Type { get; set; }
    }

    public class PackageHistoryDto
    {
        public long CreatedDate { get; set; }
        public string Status { get; set; }
    }
}
