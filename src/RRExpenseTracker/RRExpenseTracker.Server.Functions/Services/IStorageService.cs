using System.IO;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions.Services
{
    public interface IStorageService
    {
        Task<string> SaveFileAsync(Stream stream, string filename);
        Task DeleteFileAsync(string filePath);
    }
}
