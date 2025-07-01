using FluentValidation;
using Grand.Api.DTOs.Orders;
using Grand.Infrastructure.Validators;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grand.Api.Validators.Orders
{
    public class ExternalOrderRequestDtoValidator : BaseGrandValidator<OrderRequestDto>
    {
        public ExternalOrderRequestDtoValidator(IEnumerable<IValidatorConsumer<OrderRequestDto>> validators) : base(validators)
        {
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Order list (Content) cannot be empty");

            RuleForEach(x => x.Content)
                .SetValidator(new ExternalOrderDtoValidator(new List<IValidatorConsumer<ExternalOrderDto>>()));
        }
    }

    public class ExternalOrderDtoValidator : BaseGrandValidator<ExternalOrderDto>
    {
        public ExternalOrderDtoValidator(IEnumerable<IValidatorConsumer<ExternalOrderDto>> validators) : base(validators)
        {
            RuleFor(x => x.OrderNumber)
                .NotEmpty().WithMessage("OrderNumber is required");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("CustomerEmail is required")
                .EmailAddress().WithMessage("CustomerEmail must be a valid email");

            RuleFor(x => x.CustomerFirstName)
                .NotEmpty().WithMessage("CustomerFirstName is required");

            RuleFor(x => x.CustomerLastName)
                .NotEmpty().WithMessage("CustomerLastName is required");

            RuleFor(x => x.ShipmentAddress)
                .NotNull().WithMessage("ShipmentAddress is required")
                .SetValidator(new ExternalAddressDtoValidator(new List<IValidatorConsumer<OrderAddressDto>>()));

            RuleFor(x => x.InvoiceAddress)
                .NotNull().WithMessage("InvoiceAddress is required")
                .SetValidator(new ExternalAddressDtoValidator(new List<IValidatorConsumer<OrderAddressDto>>()));

            RuleFor(x => x.Lines)
                .NotEmpty().WithMessage("Order lines cannot be empty");

            RuleForEach(x => x.Lines)
                .SetValidator(new ExternalOrderLineDtoValidator(new List<IValidatorConsumer<ExternalOrderLineDto>>()));
        }
    }

    public class ExternalAddressDtoValidator : BaseGrandValidator<OrderAddressDto>
    {
        public ExternalAddressDtoValidator(IEnumerable<IValidatorConsumer<OrderAddressDto>> validators) : base(validators)
        {
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(x => x.Address1).NotEmpty().WithMessage("Address1 is required");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required");
            RuleFor(x => x.CountryCode).NotEmpty().WithMessage("CountryCode is required");
            RuleFor(x => x.FullAddress).NotEmpty().WithMessage("FullAddress is required");
        }
    }

    public class ExternalOrderLineDtoValidator : BaseGrandValidator<ExternalOrderLineDto>
    {
        public ExternalOrderLineDtoValidator(IEnumerable<IValidatorConsumer<ExternalOrderLineDto>> validators) : base(validators)
        {
            RuleFor(x => x.MerchantSku).NotEmpty().WithMessage("MerchantSku is required");
            RuleFor(x => x.Sku).NotEmpty().WithMessage("SKU is required");
            RuleFor(x => x.ProductName).NotEmpty().WithMessage("ProductName is required");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithMessage("Amount must be non-negative");
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0).WithMessage("Price must be non-negative");
        }
    }
}
