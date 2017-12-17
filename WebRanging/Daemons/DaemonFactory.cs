using System;
using WebRanging.Daemons.Analyzer;
using WebRanging.Daemons.Parser;
using WebRanging.Queue;
using WebRanging.Sites;

namespace WebRanging.Daemons
{
    public class DaemonFactory : IDaemonFactory
    {
        private readonly IQueueApi queueApi;
        private readonly ISitesApi sitesApi;

        public DaemonFactory(IQueueApi queueApi, ISitesApi sitesApi)
        {
            this.queueApi = queueApi;
            this.sitesApi = sitesApi;
        }
        
        public IDaemon New(DaemonType type)
        {
            switch (type)
            {
                case DaemonType.Parser:
                    return new ParserDaemon(queueApi, sitesApi);
                case DaemonType.Analyzer:
                    return new AnalyzerDaemon();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}