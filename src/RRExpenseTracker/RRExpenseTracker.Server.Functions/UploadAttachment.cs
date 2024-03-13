using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RRExpenseTracker.Server.Data.Interfaces;
using RRExpenseTracker.Server.Functions.Services;
using RRExpenseTracker.Shared.Responses;
using System;
using System.Net;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions
{
    public class UploadAttachment
    {
        private readonly ILogger<UploadAttachment> _logger;

        private readonly IStorageService _storageService;
        private readonly IAttachmentsRepository _attachmentsRepository;

        public UploadAttachment(ILogger<UploadAttachment> log, IStorageService storageService, IAttachmentsRepository attachmentsRepository)
        {
            _logger = log;
            _storageService = storageService;
            _attachmentsRepository = attachmentsRepository;
        }

        [FunctionName("UploadAttachment")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Upload attachment has been triggerd");

            string userId = "userId"; // TODO: Fetch from the token

            // Read the file from the form

            var file = req.Form.Files["File"];
            if (file == null)
            {
                return new BadRequestObjectResult(new ApiErrorResponse("File is required"));
            }

            // TODO: Call the Microsoft Computer Vision API to make sure it's a document image

            // Save the file and retrieve the URL
            string url = string.Empty;
            try
            {
                url = await _storageService.SaveFileAsync(file.OpenReadStream(), file.FileName);
            }
            catch (NotSupportedException)
            {

                return new BadRequestObjectResult(new ApiErrorResponse("File is required"));
            }

            // Save the URL in the database.

            await _attachmentsRepository.AddAsync(new Data.Models.Attachment()
            {
                Id = Guid.NewGuid().ToString(),
                UploadedByUserId = userId,
                UploadingDate = DateTime.UtcNow, // TODO: Use azure function datetime
                Url = url

            });

            return new OkObjectResult(new ApiSuccessResponse<string>($"Attchment has been uploaded succesfully with url {url}"));
        }
    }
}

