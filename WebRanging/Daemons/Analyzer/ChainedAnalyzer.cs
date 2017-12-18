using System;
using System.Threading;

namespace WebRanging.Daemons.Analyzer
{
    public abstract class ChainedAnalyzer : IWebometricsAnalyzer
    {
        private readonly IWebometricsAnalyzer succesor;
        public abstract WebRageType Type { get; }
        public abstract int Weight { get; }
        public int ChainWeight => Weight + (succesor?.Weight ?? 0);
        protected long Value = 0;

        protected ChainedAnalyzer(IWebometricsAnalyzer succesor)
        {
            this.succesor = succesor;
        }

        public void ProcessFile(Lazy<string> fileContent, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            ProcessFileInternal(fileContent);
            succesor?.ProcessFile(fileContent, token);
        }

        public void SaveWebometricsValue(Action<WebRageType, long> saveDelegate)
        {
            saveDelegate(Type, Value * Weight);
            succesor?.SaveWebometricsValue(saveDelegate);
        }

        protected abstract void ProcessFileInternal(Lazy<string> fileContent);
    }
}