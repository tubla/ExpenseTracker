using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RRExpenseTracker.Shared.Validators;

namespace RRExpenseTracker.Shared.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<WalletDtoValidator>();
        }
    }
}
