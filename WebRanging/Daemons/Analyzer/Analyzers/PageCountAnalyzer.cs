using System;

namespace WebRanging.Daemons.Analyzer.Analyzers
{
    public class PageCountAnalyzer : ChainedAnalyzer
    {
        public override WebRageType Type { get; } = WebRageType.Page;
        public override int Weight { get; } = 2;


        public PageCountAnalyzer(IWebometricsAnalyzer succesor) : base(succesor)
        {
        }

        protected override void ProcessFileInternal(Lazy<string> fileContent)
        {
            Value += 1;
        }
    }
}