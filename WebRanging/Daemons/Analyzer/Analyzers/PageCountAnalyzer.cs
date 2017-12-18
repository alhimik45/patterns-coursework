using WebRanging.Sites;

namespace WebRanging.Daemons.Analyzer.Analyzers
{
    public class PageCountAnalyzer : ChainedAnalyzer
    {
        public override WebRageType Type { get; } = WebRageType.Page;
        public override int Weight { get; } = 0;


        public PageCountAnalyzer(IWebometricsAnalyzer succesor) : base(succesor)
        {
        }

        protected override void ProcessFileInternal(FileInfo file)
        {
            Value += 1;
        }
    }
}