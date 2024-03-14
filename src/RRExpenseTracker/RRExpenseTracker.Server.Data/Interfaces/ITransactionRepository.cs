namespace RRExpenseTracker.Server.Data.Interfaces
{
    public interface ITransactionRepository
    {
        Task DeleteAsync(Transaction transaction);
        Task CreateAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
    }
}
