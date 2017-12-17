using System.Threading.Tasks;
using MongoDB.Driver;

namespace WebRanging.Queue
{
    public class QueueApi : IQueueApi
    {
        private readonly IMongoCollection<QueueItem> queue;

        public QueueApi(DbContext db)
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
    }
}