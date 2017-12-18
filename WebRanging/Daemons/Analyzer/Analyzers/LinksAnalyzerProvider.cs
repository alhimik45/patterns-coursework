namespace WebRanging.Daemons.Analyzer.Analyzers
{
    public class LinksAnalyzerProvider : IAnalyzerProvider
    {
        public IWebometricsAnalyzer New(IWebometricsAnalyzer succesor)
        {
            return new LinksAnalyzer(succesor);
        }
    }
}