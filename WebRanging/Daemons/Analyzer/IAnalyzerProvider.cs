namespace WebRanging.Daemons.Analyzer
{
    public interface IAnalyzerProvider
    {
        IWebometricsAnalyzer New(IWebometricsAnalyzer succesor);
    }
}