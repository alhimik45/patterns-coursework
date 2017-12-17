namespace WebRanging
{
    public class Config
    {
        public MongoSettings Mongo { get; set; }
        public ApplicationSettings Application { get; set; }

        public class MongoSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }
        
        public class ApplicationSettings
        {
            public string StoreSitesFolder { get; set; }
        }
    }
}