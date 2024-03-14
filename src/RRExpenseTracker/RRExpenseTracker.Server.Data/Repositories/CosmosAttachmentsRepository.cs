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

        public async Task<IEnumerable<Attachment>> GetUnusedAttachmentsAsync(int hours)
        {
            var queryText = "SELECT * FROM c WHERE DateTimeDiff('hour',c.uploadingDate,GetCurrentDateTime()) > @hours";
            var query = new QueryDefinition(queryText).WithParameter("@hours", hours);

            var container = _cosmosClientDb.GetContainer(DATABASE_NAME, CONTAINER_NAME);
            var iterator = container.GetItemQueryIterator<Attachment>(query);

            var result = await iterator.ReadNextAsync();

            // ContinuationToken is not null until there are records/attachments available in cosmosdb
            var attachments = new List<Attachment>();
            if (result.Any())
            {
                attachments.AddRange(result.Resource);
            }

            while (result.ContinuationToken != null) // means there are more records in the database
            {
                iterator = container.GetItemQueryIterator<Attachment>(query, result.ContinuationToken);
                result = await iterator.ReadNextAsync();
                attachments.AddRange(result.Resource);
            }

            return attachments;
        }

        public async Task DeleteAsync(string id, string uploadedByUserId)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (string.IsNullOrWhiteSpace(uploadedByUserId))
            {
                throw new ArgumentNullException(nameof(uploadedByUserId));
            }

            var container = _cosmosClientDb.GetContainer(DATABASE_NAME, CONTAINER_NAME);
            await container.DeleteItemAsync<Attachment>(id, new PartitionKey(uploadedByUserId));
        }

        public async Task DeleteBatchAsync(IEnumerable<Attachment> attachments)
        {
            if (attachments == null)
            {
                throw new ArgumentNullException(nameof(attachments));
            }

            if (!attachments.Any())
            {
                return;
            }

            var container = _cosmosClientDb.GetContainer(DATABASE_NAME, CONTAINER_NAME);
            var tasks = new List<Task>();
            foreach (var item in attachments)
            {
                tasks.Add(container.DeleteItemAsync<Attachment>(item.Id, new PartitionKey(item.UploadedByUserId)));
            }

            await Task.WhenAll(tasks);
        }

        public async Task<IEnumerable<Attachment>> GetByUrlsAsync(string[] urls)
        {
            if (urls == null)
            {
                throw new ArgumentNullException(nameof(urls));
            }

            if (!urls.Any())
            {
                return Enumerable.Empty<Attachment>();
            }

            //SELECT * FROM c WHERE c.url in ('https://l...png','https://h...jpg')
            var urlAsString = string.Join(",", urls.Select(x => $"'{x}'"));

            var queryText = $"SELECT * FROM c WHERE c.url in (@urls)";
            var query = new QueryDefinition(queryText).WithParameter("@urls", urlAsString);

            var container = _cosmosClientDb.GetContainer(DATABASE_NAME, CONTAINER_NAME);
            var iterator = container.GetItemQueryIterator<Attachment>(query);

            var result = await iterator.ReadNextAsync();

            if (result.Any())
            {
                return result.Resource;
            }
            return Enumerable.Empty<Attachment>();
        }
    }
}
