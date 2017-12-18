using System;
using System.Threading;
using WebRanging.Sites;

namespace WebRanging.Daemons.Analyzer
{
    public interface IWebometricsAnalyzer
    {
        WebRageType Type { get; }
        int Weight { get; }
        int ChainWeight { get; }
        void ProcessFile(FileInfo file, CancellationToken token);
        void SaveWebometricsValue(Action<WebRageType, long, int> saveDelegate);
    }
}