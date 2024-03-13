using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using RRExpenseTracker.Server.Data.Extensions;
using RRExpenseTracker.Server.Functions.Extensions;
using RRExpenseTracker.Shared.Extensions;

[assembly: FunctionsStartup(typeof(RRExpenseTracker.Server.Functions.Startup))]
namespace RRExpenseTracker.Server.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var configuration = builder.GetContext().Configuration;

            builder.Services.AddCosmosDbClient(configuration["CosmosDb_ConnectionString"]);
            builder.Services.AddRepositories();
            builder.Services.AddValidators();
            builder.Services.AddStorageService(configuration["AzureWebJobsStorage"]);
            builder.Services.AddComputerVisionService(configuration["ComputerVisionApiKey"], configuration["ComputerVisionEndpoint"]);

        }
    }
}
