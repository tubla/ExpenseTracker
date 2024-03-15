using Newtonsoft.Json;

namespace RRExpenseTracker.Server.Data.Models
{
    public class Transaction
    {

        [JsonProperty("id")]
        public string Id { get; private set; } = Guid.NewGuid().ToString();

        [JsonProperty("description")]
        public string? Description { get; private set; }

        [JsonProperty("isIncome")]
        public bool IsIncome { get; private set; }

        [JsonProperty("amount")]
        public double Amount { get; private set; }

        [JsonProperty("creationDate")]
        public DateTime CreationDate { get; private set; } = DateTime.UtcNow;

        [JsonProperty("modificationDate")]
        public DateTime? ModificationDate { get; private set; }

        [JsonProperty("category")]
        public string Category { get; private set; } = string.Empty;

        [JsonProperty("tags")]
        public string[]? Tags { get; private set; }

        [JsonProperty("attachments")]
        public string[]? Attachments { get; private set; }

        private string _userId = string.Empty;

        [JsonProperty("userId")]
        public string UserId
        {
            get => _userId;
            private set
            {
                _userId = value;
                UserIdYear = $"{_userId}-{CreationDate.Year}";
            }
        }

        [JsonProperty("userIdYear")]
        public string UserIdYear { get; private set; } = string.Empty;

        [JsonProperty("walletId")]
        public string WalletId { get; private set; } = string.Empty;



        public static Transaction Create(string walletId,
                                         string userId,
                                         double amount,
                                         string category,
                                         bool isIncome,
                                         string? description = null,
                                         string[]? tags = null,
                                         string[]? attachments = null)
        {
            return new Transaction
            {
                WalletId = walletId,
                UserId = userId,
                Amount = amount,
                Category = category,
                IsIncome = isIncome,
                Description = description,
                Tags = tags,
                Attachments = attachments,
                ModificationDate = DateTime.UtcNow
            };
        }

        public void Update(bool isIncome,
                           double amount,
                           string category,
                           string? description = null,
                           string[]? tags = null,
                           string[]? attachments = null)
        {
            IsIncome = isIncome;
            Amount = amount;
            Category = category;
            Description = description;
            Tags = tags;
            Attachments = attachments;
        }

    }
}
