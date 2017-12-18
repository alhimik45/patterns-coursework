using System;
using System.Threading;

namespace WebRanging.Daemons.Analyzer
{
    public interface IWebometricsAnalyzer
    {
        WebRageType Type { get; }
        int Weight { get; }
        int ChainWeight { get; }
        void ProcessFile(Lazy<string> fileContent, CancellationToken token);
        void SaveWebometricsValue(Action<WebRageType, long> saveDelegate);
    }
}