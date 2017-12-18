namespace WebRanging.Daemons.Analyzer.Analyzers
{
    public class PageCountAnalyzerProvider : IAnalyzerProvider
    {
        public IWebometricsAnalyzer New(IWebometricsAnalyzer succesor)
        {
            return new PageCountAnalyzer(succesor);
        }
    }
}