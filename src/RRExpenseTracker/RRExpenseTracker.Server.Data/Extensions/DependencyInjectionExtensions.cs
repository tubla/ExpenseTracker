using Microsoft.Extensions.DependencyInjection;

namespace RRExpenseTracker.Server.Data.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddCosmosDbClient(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(sp => new CosmosClient(connectionString));
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IWalletRepository, CosmosWalletsRepository>();
        }
    }
}
