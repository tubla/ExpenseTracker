using FluentValidation;
using RRExpenseTracker.Shared.DTOs;

namespace RRExpenseTracker.Shared.Validators
{
    public class WalletDtoValidator : AbstractValidator<WalletDto>
    {
        public WalletDtoValidator()
        {

            RuleFor(p => p.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MaximumLength(100)
                .WithMessage("Name must be less than 100 characters");

            RuleFor(p => p.Currency)
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(3)
                .WithMessage("Currency must be 3 characters length");

            RuleFor(p => p.Swift)
                .Length(8, 12)
                .When(p => !string.IsNullOrWhiteSpace(p.Swift))
                .WithMessage("Swift must be between 8-12 characters length");

            RuleFor(p => p.Iban)
                .MaximumLength(34)
                .When(p => !string.IsNullOrWhiteSpace(p.Iban))
                .WithMessage("IBan must be less than 34 characters length");
        }
    }
}
