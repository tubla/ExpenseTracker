using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions.Services
{
    public interface IImageAnalyzerService
    {
        Task<IEnumerable<string>> ExtractImageCatagoriesAsync(Stream imageStream);
    }
}
