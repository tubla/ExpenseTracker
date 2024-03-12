using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RRExpenseTracker.Server.Data.Interfaces;
using RRExpenseTracker.Shared.DTOs;
using RRExpenseTracker.Shared.Responses;
using System.Net;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions
{
    public class GetWalletDetails
    {
        private readonly ILogger<GetWalletDetails> _logger;

        private readonly IWalletRepository _walletRepository;

        public GetWalletDetails(ILogger<GetWalletDetails> log, IWalletRepository walletRepository)
        {
            _logger = log;
            _walletRepository = walletRepository;
        }

        [FunctionName("GetWalletDetails")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            // TODO: Fetch the user id from the access token
            string userId = "userId";
            string walletId = req.Query["id"];
            _logger.LogInformation($"Retrieve the wallet with id {walletId} for the user {userId}");

            if (string.IsNullOrWhiteSpace(walletId))
            {
                return new BadRequestObjectResult(new ApiErrorResponse("Wallet id is required"));
            }

            var wallet = await _walletRepository.GetByIdAsync(walletId, userId);
            if (wallet == null)
            {
                return new NotFoundResult();
            }


            return new OkObjectResult(new WalletDto
            {
                Id = wallet.Id,
                Name = wallet.Name,
                AccountType = wallet.AccountType,
                Balance = wallet.Balance,
                Currency = wallet.Currency,
                BankName = wallet.BankName,
                CreationDate = wallet.CreationDate,
                Iban = wallet.Iban,
                Swift = wallet.Swift,
                Type = wallet.Type.Value,
                Username = wallet.Username
            });
        }
    }
}

