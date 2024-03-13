using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using RRExpenseTracker.Server.Data.Extensions;
using RRExpenseTracker.Server.Functions.Services;
using RRExpenseTracker.Shared.Extensions;

[assembly: FunctionsStartup(typeof(RRExpenseTracker.Server.Functions.Startup))]
namespace RRExpenseTracker.Server.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddCosmosDbClient(builder.GetContext().Configuration["CosmosDb_ConnectionString"]);
            builder.Services.AddRepositories();
            builder.Services.AddValidators();
            builder.Services.AddScoped<IStorageService>(sp => new AzureBlobStorageService(builder.GetContext().Configuration["AzureWebJobsStorage"]));
        }
    }
}
