using RRExpenseTracker.Server.Data.Models;

namespace RRExpenseTracker.Server.Data.Interfaces
{
    public interface IWalletRepository
    {
        Task<IEnumerable<Wallet>> ListByUserIdAsync(string userId);
    }
}
