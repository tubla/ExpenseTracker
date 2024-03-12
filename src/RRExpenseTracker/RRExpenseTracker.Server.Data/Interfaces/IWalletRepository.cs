namespace RRExpenseTracker.Server.Data.Interfaces
{
    public interface IWalletRepository
    {
        Task<IEnumerable<Wallet>> ListByUserIdAsync(string userId);
        Task<Wallet?> GetByIdAsync(string walletId, string userId);
    }
}
