using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace WebRanging.Queue
{
    public class QueueApi : IQueueApi
    {
        private readonly IMongoCollection<QueueItem> queue;

        public QueueApi(IDbContext db)
        {
            queue = db.Queue;
        }

        public Task Add(QueueItem item)
        {
            return queue.InsertOneAsync(item);
        }

        public Task<QueueItem> Fetch(JobType jobType)
        {
            return queue.FindOneAndDeleteAsync<QueueItem>(
                item => item.JobType == jobType,
                new FindOneAndDeleteOptions<QueueItem>
                {
                    Sort = new SortDefinitionBuilder<QueueItem>()
                        .Ascending(item => item.AdditionTime)
                });
        }

        public async Task<List<QueueItem>> GetList(JobType jobType)
        {
            var list = await queue.FindAsync(
                item => item.JobType == jobType,
                new FindOptions<QueueItem>
                {
                    Sort = new SortDefinitionBuilder<QueueItem>()
                        .Ascending(item => item.AdditionTime)
                });
            return list.ToList();
        }
    }
}