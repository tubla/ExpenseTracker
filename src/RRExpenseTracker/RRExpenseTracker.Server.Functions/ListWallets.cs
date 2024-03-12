using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using RRExpenseTracker.Server.Data.Interfaces;
using RRExpenseTracker.Server.Data.Models;
using RRExpenseTracker.Shared.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RRExpenseTracker.Server.Functions
{
    public class ListWallets
    {
        private readonly ILogger<ListWallets> _logger;
        private readonly IWalletRepository _walletRepository;
        public ListWallets(ILogger<ListWallets> log, IWalletRepository walletRepository)
        {
            _logger = log;
            _walletRepository = walletRepository;
        }

        [FunctionName("ListWallets")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "name" })]
        [OpenApiSecurity("function_key", SecuritySchemeType.ApiKey, Name = "code", In = OpenApiSecurityLocationType.Query)]
        [OpenApiParameter(name: "name", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Name** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {


            string userId = "userId";
            var wallets = await _walletRepository.ListByUserIdAsync(userId);
            return new OkObjectResult(new ApiSuccessResponse<IEnumerable<Wallet>>($"{wallets.Count()} wallets have been retrieved", wallets));
        }
    }
}

