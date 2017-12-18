namespace WebRanging.Daemons.Analyzer.Analyzers
{
    public class ParagraphAnalyzerProvider : IAnalyzerProvider
    {
        public IWebometricsAnalyzer New(IWebometricsAnalyzer succesor)
        {
            return new ParagraphAnalyzer(succesor);
        }
    }
}