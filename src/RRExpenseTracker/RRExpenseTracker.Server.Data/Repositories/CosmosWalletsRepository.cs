namespace RRExpenseTracker.Server.Data.Repositories
{
    public class CosmosWalletsRepository : IWalletRepository
    {

        private readonly CosmosClient _cosmosClientDb;
        private const string DATABASE_NAME = "ExpenseTrackerDb";
        private const string CONTAINER_NAME = "Wallets";


        public CosmosWalletsRepository(CosmosClient cosmosClientDb)
        {
            _cosmosClientDb = cosmosClientDb;
        }

        public async Task<IEnumerable<Wallet>> ListByUserIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException(nameof(User));
            }

            var queryText = $"SELECT * FROM c WHERE c.userId = @userId";
            var query = new QueryDefinition(queryText).WithParameter("@userId", userId);

            var container = _cosmosClientDb.GetContainer(DATABASE_NAME, CONTAINER_NAME);
            var iterator = container.GetItemQueryIterator<Wallet>(query);

            var result = await iterator.ReadNextAsync();

            return result.Resource;

        }
    }
}
