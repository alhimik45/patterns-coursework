using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<IAnalyzerProvider> analyzerProviders;

        public DaemonFactory(IQueueApi queueApi, ISitesApi sitesApi, IEnumerable<IAnalyzerProvider> analyzerProviders)
        {
            this.queueApi = queueApi;
            this.sitesApi = sitesApi;
            this.analyzerProviders = analyzerProviders.ToList();
        }
        
        public IDaemon New(DaemonType type)
        {
            switch (type)
            {
                case DaemonType.Parser:
                    return new ParserDaemon(queueApi, sitesApi);
                case DaemonType.Analyzer:
                    return new AnalyzerDaemon(queueApi, sitesApi, analyzerProviders);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}