namespace RRExpenseTracker.Shared.Responses
{
    public class ApiResponse
    {
        public string? Message { get; set; }
        public DateTime ResponseDate { get; set; }

        public bool IsSuccess { get; set; }
    }
}
