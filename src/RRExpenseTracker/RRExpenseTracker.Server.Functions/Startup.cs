using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using RRExpenseTracker.Server.Data.Extensions;

[assembly: FunctionsStartup(typeof(RRExpenseTracker.Server.Functions.Startup))]
namespace RRExpenseTracker.Server.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddCosmosDbClient(builder.GetContext().Configuration["CosmosDb_ConnectionString"]);
            builder.Services.AddRepositories();
        }
    }
}
