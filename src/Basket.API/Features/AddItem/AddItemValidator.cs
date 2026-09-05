using FluentValidation;

namespace Basket.API.Features.AddItem
{
    public class AddItemValidator : AbstractValidator<AddItemCommand>
    {
        public AddItemValidator()
        {
            RuleFor(i => i.Quantity)
                .NotNull()
                .GreaterThan(0);

            RuleFor(i => i.ProductId)
                .NotNull();
        }
    }
}
