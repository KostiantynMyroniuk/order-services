using FluentValidation;
using Orders.API.Models;

namespace Orders.API.Features.CreateOrder
{
    public class CreateOrderValidation : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderValidation()
        {
            RuleFor(o => o.RequestId)
                .NotEmpty();

            RuleFor(o => o.Address)
                .NotNull()
                .SetValidator(new AddressValidator());

            RuleFor(o => o.Items)
                .NotEmpty()
                .NotNull();

            RuleForEach(o => o.Items)
                .SetValidator(new OrderItemRequestValidator());
        }
    }

    public class AddressValidator : AbstractValidator<Address>
    {
        public AddressValidator()
        {
            RuleFor(a => a.Country)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(a => a.City)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(a => a.Street)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(a => a.ZipCode)
                .NotEmpty()
                .Matches(@"^\d{5}$").WithMessage("Zipcode should contain 5 numbers");
        }
    }

    public class OrderItemRequestValidator : AbstractValidator<OrderItemRequest>
    {
        public OrderItemRequestValidator()
        {
            RuleFor(i => i.ProductId)
                .NotEmpty();

            RuleFor(i => i.Quantity)
                .GreaterThan(0);
        }
    }
}
