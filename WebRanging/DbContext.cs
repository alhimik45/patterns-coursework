using MongoDB.Driver;
using WebRanging.Daemons;
using WebRanging.Queue;
using WebRanging.Sites;

namespace WebRanging
{
    public class DbContext : IDbContext
    {
        public IMongoCollection<QueueItem> Queue => Database.GetCollection<QueueItem>("queue");
        public IMongoCollection<Site> Sites => Database.GetCollection<Site>("sites");
        private IMongoDatabase Database { get; }

        public DbContext(string connectionString, string databaseName)
        {
            Database = new MongoClient(connectionString).GetDatabase(databaseName);
        }
    }
}