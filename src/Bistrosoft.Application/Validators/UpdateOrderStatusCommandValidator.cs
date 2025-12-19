using Bistrosoft.Application.Commands;
using FluentValidation;

namespace Bistrosoft.Application.Validators;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId)
            .NotEmpty().WithMessage("Order ID is required.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid OrderStatus value.");
    }
}



