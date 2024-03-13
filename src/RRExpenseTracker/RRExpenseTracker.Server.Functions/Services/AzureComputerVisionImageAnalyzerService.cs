using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions.Services
{
    public class AzureComputerVisionImageAnalyzerService : IImageAnalyzerService
    {
        private readonly ComputerVisionClient _computerVisionClient;

        public AzureComputerVisionImageAnalyzerService(ComputerVisionClient computerVisionClient)
        {
            _computerVisionClient = computerVisionClient;
        }

        public async Task<IEnumerable<string>> ExtractImageCatagoriesAsync(Stream imageStream)
        {
            var requiredFeatures = new List<VisualFeatureTypes?>() { VisualFeatureTypes.Categories };
            var result = await _computerVisionClient.AnalyzeImageInStreamAsync(imageStream, requiredFeatures);
            return result.Categories.Select(c => c.Name);
        }
    }
}
