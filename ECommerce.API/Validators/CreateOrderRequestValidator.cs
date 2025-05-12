using ECommerce.API.Dtos;
using ECommerce.Application.Commands;
using FluentValidation;

namespace ECommerce.API.Validators
{
    public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequestDto>
    {
        public CreateOrderRequestValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required.");

            RuleFor(x => x.Products).NotEmpty().WithMessage("Products list cannot be empty.");

            RuleForEach(x => x.Products).ChildRules(product =>
            {
                product.RuleFor(p => p.ProductId).NotEmpty().WithMessage("ProductId is required.");

                product.RuleFor(p => p.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0.");
            });
        }
    }
}
