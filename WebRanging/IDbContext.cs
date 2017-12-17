using MongoDB.Driver;
using WebRanging.Daemons;
using WebRanging.Queue;
using WebRanging.Sites;

namespace WebRanging
{
    public interface IDbContext
    {
        IMongoCollection<QueueItem> Queue { get; }
        IMongoCollection<Site> Sites { get; }
    }
}