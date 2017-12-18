using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebRanging.Daemons.Analyzer.Analyzers
{
    public class ParagraphAnalyzer : ChainedAnalyzer
    {
        public override WebRageType Type { get; } = WebRageType.BigText;
        public override int Weight { get; } = 2;

        private static readonly Regex ParagraphRegex = new Regex(@"<(p|pre|dd)>.*?</?\1>", RegexOptions.Singleline);

        private static readonly Regex TagRegex = new Regex(@"<.*?>");

        public ParagraphAnalyzer(IWebometricsAnalyzer succesor) : base(succesor)
        {
        }

        protected override void ProcessFileInternal(Lazy<string> fileContent)
        {
            Value += ParagraphRegex
                .Matches(fileContent.Value)
                .Select(m => TagRegex.Replace(m.Captures[0].Value, ""))
                .Count(s => s.Length > 200);
        }
    }
}