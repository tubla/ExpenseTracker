namespace RRExpenseTracker.Shared.Responses
{
    public class ApiErrorResponse<T> : ApiResponse
    {
        public ApiErrorResponse()
        {
            IsSuccess = false;
            ResponseDate = DateTime.UtcNow;
        }

        public ApiErrorResponse(string message) : this()
        {
            Message = message;
        }
    }
}
