namespace RRExpenseTracker.Shared.DTOs
{
    public class TransactionDto
    {
        public string? Id { get; set; }
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime? DateTime { get; set; }
        public string Category { get; set; } = "Other";
        public string[]? Tags { get; set; }
        public string[]? Attachments { get; set; }
        public bool IsIncome { get; set; }
        public string WalletId { get; set; } = string.Empty;
    }
}
