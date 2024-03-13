using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using RRExpenseTracker.Server.Data.Interfaces;
using RRExpenseTracker.Server.Functions.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions
{
    public class DeleteUnusedAttachments
    {
        private readonly IStorageService _storageService;
        private readonly IAttachmentsRepository _attachmentsRepository;

        public DeleteUnusedAttachments(IStorageService storageService, IAttachmentsRepository attachmentsRepository)
        {
            _storageService = storageService;
            _attachmentsRepository = attachmentsRepository;
        }

        [FunctionName("DeleteUnusedAttachments")]
        public async Task Run([TimerTrigger("0 0 */6 ? * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"Delete unused attachments triggered at: {DateTime.Now}");

            // Retrieve attachments to be deleted
            var attachments = await _attachmentsRepository.GetUnusedAttachmentsAsync(1);
            if (attachments.Any())
            {
                int totalAttachments = attachments.Count();
                int deletedCount = 0;

                log.LogInformation($"{totalAttachments} attachments to be deleted");

                foreach (var item in attachments)
                {

                    try
                    {
                        //First remove it from the blob storage
                        await _storageService.DeleteFileAsync(item.Url);
                        await _attachmentsRepository.DeleteAsync(item.Id, item.UploadedByUserId);
                        deletedCount++;
                    }
                    catch (Exception)
                    {
                        var fileName = Path.GetFileName(item.Url);
                        log.LogError($"ERROR - unable to delete file {fileName}");
                    }
                }

                log.LogInformation($"{deletedCount}/{totalAttachments} attachments have been deleted successfully");
            }
            else
            {
                log.LogInformation("No attachments to be deleted have been found");
            }
        }
    }
}
