using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.DependencyInjection;
using RRExpenseTracker.Server.Functions.Services;

namespace RRExpenseTracker.Server.Functions.Extensions
{
    public static class AddServicesExtensions
    {
        public static void AddComputerVisionService(this IServiceCollection services, string apiKey, string endpoint)
        {
            services.AddScoped(sp => new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey))
            {
                Endpoint = endpoint
            });

            services.AddScoped<IImageAnalyzerService, AzureComputerVisionImageAnalyzerService>();
        }

        public static void AddStorageService(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IStorageService>(sp => new AzureBlobStorageService(connectionString));
        }
    }
}
