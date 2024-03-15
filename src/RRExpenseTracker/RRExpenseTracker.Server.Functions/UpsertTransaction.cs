using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using RRExpenseTracker.Server.Data.Interfaces;
using RRExpenseTracker.Server.Data.Models;
using RRExpenseTracker.Shared.DTOs;
using RRExpenseTracker.Shared.Responses;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions
{
    public class UpsertTransaction
    {
        private readonly ILogger<UpsertTransaction> _logger;
        private readonly IAttachmentsRepository _attachmentsRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IValidator<TransactionDto> _validator;

        public UpsertTransaction(ILogger<UpsertTransaction> log,
                                 IAttachmentsRepository attachmentsRepository,
                                 ITransactionRepository transactionRepository,
                                 IValidator<TransactionDto> validator,
                                 IWalletRepository walletRepository)
        {
            _logger = log;
            _attachmentsRepository = attachmentsRepository;
            _transactionRepository = transactionRepository;
            _validator = validator;
            _walletRepository = walletRepository;
        }

        [FunctionName("UpsertTransaction")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("Upsert transaction function triggered");

            var userId = "userId";

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<TransactionDto>(requestBody);

            var isUpdate = !string.IsNullOrEmpty(data.Id);
            if (isUpdate)
            {

                return new OkResult();
            }
            else
            {
                var validationResult = _validator.Validate(data);
                if (!validationResult.IsValid)
                {
                    return new BadRequestObjectResult(new ApiErrorResponse("Invalid data", validationResult.Errors.Select(e => e.ErrorMessage)));
                }

                var wallet = await _walletRepository.GetByIdAsync(data.WalletId, userId);
                if (wallet == null)
                {
                    return new BadRequestObjectResult(new ApiErrorResponse("Wallet not found"));
                }

                IEnumerable<Attachment> attachments = new List<Attachment>();
                if (data.Attachments != null && data.Attachments.Any())
                {
                    attachments = await _attachmentsRepository.GetByUrlsAsync(data.Attachments);
                    if (data.Attachments.Distinct().Count() != attachments.Count())
                    {
                        return new BadRequestObjectResult(new ApiErrorResponse("Invalid attachments"));
                    }
                }

                var transaction = Transaction.Create(wallet.Id,
                                                     userId,
                                                     data.Amount,
                                                     data.Category,
                                                     data.IsIncome,
                                                     data.Description,
                                                     data.Tags,
                                                     attachments?.Select(a => a.Url).ToArray());

                await _transactionRepository.CreateAsync(transaction);

                var amountToAdd = data.IsIncome ? data.Amount : -data.Amount;
                await _walletRepository.UpdateBalanceAsync(wallet.Id, userId, amountToAdd);

                data.Id = transaction.Id;
                return new OkObjectResult(new ApiSuccessResponse<TransactionDto>("Transaction created", data));
            }
        }
    }
}

