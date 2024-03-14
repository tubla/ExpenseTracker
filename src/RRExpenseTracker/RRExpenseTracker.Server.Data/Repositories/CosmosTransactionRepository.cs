namespace RRExpenseTracker.Server.Data.Repositories
{
    public class CosmosTransactionRepository : ITransactionRepository
    {

        private readonly CosmosClient _cosmosClientDb;
        private const string DATABASE_NAME = "ExpenseTrackerDb";
        private const string CONTAINER_NAME = "Transactions";

        private readonly Container _container;


        public CosmosTransactionRepository(CosmosClient cosmosClientDb)
        {
            _cosmosClientDb = cosmosClientDb;
            _container = _cosmosClientDb.GetContainer(DATABASE_NAME, CONTAINER_NAME);
        }

        public async Task CreateAsync(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            await _container.CreateItemAsync(transaction);
        }

        public async Task DeleteAsync(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            await _container.DeleteItemAsync<Transaction>(transaction.Id, new PartitionKey(transaction.UserIdYear));
        }

        public async Task UpdateAsync(Transaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            await _container.ReplaceItemAsync<Transaction>(transaction, transaction.Id);
        }
    }
}
