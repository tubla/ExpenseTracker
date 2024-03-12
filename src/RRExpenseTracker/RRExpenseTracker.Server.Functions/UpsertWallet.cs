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
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions
{
    public class UpsertWallet
    {
        private readonly ILogger<UpsertWallet> _logger;
        private readonly IWalletRepository _walletRepository;
        private readonly IValidator<WalletDto> _walletValidator;

        public UpsertWallet(ILogger<UpsertWallet> log, IWalletRepository walletRepository, IValidator<WalletDto> walletValidator)
        {
            _logger = log;
            _walletRepository = walletRepository;
            _walletValidator = walletValidator;
        }

        [FunctionName("UpsertWallet")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = null)] HttpRequest req)
        {



            _logger.LogInformation("Upsert wallet started");

            string userId = "userId"; //TODO: Fetch it from the access token

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var data = JsonConvert.DeserializeObject<WalletDto>(requestBody);


            var validationResult = _walletValidator.Validate(data);
            if (!validationResult.IsValid)
            {
                _logger.LogError("ERROR - Upsert wallet invalid input");
                return new BadRequestObjectResult(new ApiErrorResponse("Wallet inputs are not valid", validationResult.Errors.Select(e => e.ErrorMessage)));
            }

            // TODO: Validate the number of wallets based on the subscription type


            Wallet wallet = null;
            var isAdd = string.IsNullOrWhiteSpace(data.Id);

            // Add opertaion
            if (isAdd)
            {
                wallet = new()
                {
                    CreationDate = DateTime.UtcNow,
                    Id = Guid.NewGuid().ToString()
                };
            }
            else // update operation
            {
                wallet = await _walletRepository.GetByIdAsync(data.Id, userId);
                if (wallet == null)
                {
                    _logger.LogError("ERROR - wallet not found");
                    return new NotFoundObjectResult(new ApiErrorResponse("Wallet not found"));
                }
            }


            wallet.Name = data.Name;
            wallet.AccountType = data.AccountType;
            wallet.Swift = data.Swift;
            wallet.BankName = data.BankName;
            wallet.Currency = data.Currency;
            wallet.Iban = data.Iban;
            wallet.ModificationDate = DateTime.UtcNow;
            wallet.UserId = userId;
            wallet.Username = data.Username;
            wallet.WalletTypeName = data.Type.ToString();


            if (isAdd)
            {
                await _walletRepository.CreateAsync(wallet);
            }
            else
            {
                await _walletRepository.UpdateAsync(wallet);
            }


            // Set the auto generated properties

            data.Id = wallet.Id;
            data.CreationDate = wallet.CreationDate;


            return new OkObjectResult(new ApiSuccessResponse<WalletDto>($"Wallet {(isAdd ? "inserted" : "updated")} successfully", data));
        }
    }
}

