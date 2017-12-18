using System;
using System.Linq;
using System.Text.RegularExpressions;
using WebRanging.Sites;

namespace WebRanging.Daemons.Analyzer.Analyzers
{
    public class LinksAnalyzer : ChainedAnalyzer
    {
        public override WebRageType Type { get; } = WebRageType.Links;
        public override int Weight { get; } = 1;

        private static readonly Regex LinkRegex = new Regex(@"<a.*?href=""(http.*?)"".*?>", RegexOptions.Singleline);

        public LinksAnalyzer(IWebometricsAnalyzer succesor) : base(succesor)
        {
        }

        protected override void ProcessFileInternal(FileInfo file)
        {
            Value += LinkRegex
                .Matches(file.Content.Value)
                .Select(m => m.Groups[1].Captures[0].Value)
                .Count(link => Uri.TryCreate(link, UriKind.Absolute, out var u) &&
                               DaemonUtils.GetRootDomain(u.Host) != DaemonUtils.GetRootDomain(file.OwnerHost));
        }
    }
}