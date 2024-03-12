namespace RRExpenseTracker.Server.Data.Repositories
{
    public class CosmosAttachmentsRepository : IAttachmentsRepository
    {
        private readonly CosmosClient _cosmosClientDb;
        private const string DATABASE_NAME = "ExpenseTrackerDb";
        private const string CONTAINER_NAME = "Attachments";


        public CosmosAttachmentsRepository(CosmosClient cosmosClientDb)
        {
            _cosmosClientDb = cosmosClientDb;
        }

        public async Task AddAsync(Attachment attachment)
        {
            if (attachment == null)
            {
                throw new ArgumentNullException(nameof(attachment));
            }

            var container = _cosmosClientDb.GetContainer(DATABASE_NAME, CONTAINER_NAME);
            await container.CreateItemAsync(attachment);
        }
    }
}
