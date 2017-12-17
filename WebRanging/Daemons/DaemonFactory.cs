using System;
using WebRanging.Daemons.Analyzer;
using WebRanging.Daemons.Parser;

namespace WebRanging.Daemons
{
    public class DaemonFactory : IDaemonFactory
    {
        public IDaemon New(DaemonType type)
        {
            switch (type)
            {
                case DaemonType.Parser:
                    return new ParserDaemon();
                case DaemonType.Analyzer:
                    return new AnalyzerDaemon();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}