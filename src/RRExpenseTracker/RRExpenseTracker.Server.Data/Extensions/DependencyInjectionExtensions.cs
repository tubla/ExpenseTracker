using Microsoft.Extensions.DependencyInjection;

namespace RRExpenseTracker.Server.Data.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static void AddCosmosDbClient(this IServiceCollection services, string connectionString)
        {
            services.AddSingleton(sp => new CosmosClient(connectionString, new CosmosClientOptions
            {
                AllowBulkExecution = true
            }));
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IWalletRepository, CosmosWalletsRepository>();
            services.AddScoped<IAttachmentsRepository, CosmosAttachmentsRepository>();
            services.AddScoped<ITransactionRepository, CosmosTransactionRepository>();
        }
    }
}
