using Newtonsoft.Json;

namespace RRExpenseTracker.Server.Data.Models
{
    public class Wallet
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("typeName")]
        public string? TypeName { get; set; }

        [JsonProperty("type")]
        public WalletType? Type => GetWalletTypeFromString(TypeName);


        [JsonProperty("bankName")]
        public string? BankName { get; set; }

        [JsonProperty("name")]
        public string? Name { get; set; }

        [JsonProperty("iban")]
        public string? Iban { get; set; }

        [JsonProperty("accountType")]
        public string? AccountType { get; set; }

        [JsonProperty("userId")]
        public string? UserId { get; set; }

        [JsonProperty("swift")]
        public string? Swift { get; set; }

        [JsonProperty("balance")]
        public decimal Balance { get; set; }

        [JsonProperty("currency")]
        public string? Currency { get; set; }

        [JsonProperty("username")]
        public string? Username { get; set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; set; }

        [JsonProperty("modificationDate")]
        public DateTime ModificationDate { get; set; }

        private WalletType? GetWalletTypeFromString(string? typeName)
        {
            return typeName switch
            {
                "Bank" => WalletType.Bank,
                "PayPal" => WalletType.PayPal,
                "Cash" => WalletType.Cash,
                _ => WalletType.Other
            };
        }
    }
}
