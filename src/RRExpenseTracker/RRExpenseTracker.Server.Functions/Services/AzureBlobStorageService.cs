using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions.Services
{
    public class AzureBlobStorageService : IStorageService
    {
        private readonly string _connectionString;

        public AzureBlobStorageService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task DeleteFileAsync(string filePath)
        {
            var container = await GetContainerAsync();
            var filename = Path.GetFileName(filePath);

            var blob = container.GetBlobClient(filename);

            await blob.DeleteIfExistsAsync();

        }

        public async Task<string> SaveFileAsync(Stream stream, string filename)
        {
            var container = await GetContainerAsync();

            // Retrieve and validate the extension
            var extension = Path.GetExtension(filename).ToLower();


            // Generate a new unique name
            var nameOnly = Path.GetFileNameWithoutExtension(filename);
            var newFileName = $"{nameOnly}-{Guid.NewGuid()}{extension}";

            // Upload to the blob storage account
            var blob = container.GetBlobClient(newFileName);
            await blob.UploadAsync(stream, new BlobUploadOptions
            {
                // Set the content-type of the blob, this is important for the browser as browser
                // checks the content-type to display the content in the browser when retrieved through url.  
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = GetContentType(extension)
                }
            });


            return blob.Uri.AbsoluteUri;
        }

        private async Task<BlobContainerClient> GetContainerAsync()
        {
            var blobClient = new BlobServiceClient(_connectionString);
            var container = blobClient.GetBlobContainerClient("attachments");
            await container.CreateIfNotExistsAsync();
            return container;
        }

        private string GetContentType(string extension)
        {
            return extension switch
            {
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => throw new NotSupportedException($"The extension {extension} is not supported")

            };
        }
    }
}
