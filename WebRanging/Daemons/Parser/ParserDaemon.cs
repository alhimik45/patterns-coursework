using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebRanging.Daemons.Analyzer;
using WebRanging.Queue;
using WebRanging.Sites;

namespace WebRanging.Daemons.Parser
{
    public class ParserDaemon : TaskDaemon
    {
        private readonly IQueueApi queueApi;
        private readonly ISitesApi sitesApi;
        public override DaemonType Type { get; } = DaemonType.Parser;

        private static readonly Regex UrlRegex = new Regex(@"href=""([^""]+)""");

        public ParserDaemon(IQueueApi queueApi, ISitesApi sitesApi)
        {
            this.queueApi = queueApi;
            this.sitesApi = sitesApi;
        }

        protected override Task<QueueItem> GetTask() => queueApi.Fetch(JobType.ParseSite);

        protected override async Task ExecuteIteration(QueueItem task, CancellationToken token)
        {
            try
            {
                var url = task.Arguments["url"];
                if (!DaemonUtils.IsUrl(url))
                {
                    return;
                }

                var depthStr = task.Arguments.GetValueOrDefault("parseDepth", "5");
                if (!uint.TryParse(depthStr, out var depth))
                {
                    depth = 5;
                }

                var uri = new Uri(url);
                var siteId = await sitesApi.NewSite(uri.Host);
                status = $"Скачивание {uri.Host}";
                try
                {
                    await Parse(siteId, uri, 0, depth, new ConcurrentDictionary<int, bool>(), token);
                }
                catch (OperationCanceledException)
                {
                    await queueApi.Add(new QueueJobBuilder().OfParsing(url).ParsingDepth(depth).Build());
                    throw;
                }

                if (token.IsCancellationRequested)
                {
                    await queueApi.Add(new QueueJobBuilder().OfParsing(url).ParsingDepth(depth).Build());
                    token.ThrowIfCancellationRequested();
                }

                await queueApi.Add(new QueueJobBuilder().OfAnalyze(siteId, uri.Host).Build());
                status = DaemonConstants.IdleStatus;
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        private async Task Parse(string siteId, Uri uri, uint depth, uint maxDepth,
            ConcurrentDictionary<int, bool> parsed, CancellationToken token)
        {
            if (token.IsCancellationRequested)
            {
                return;
            }

            var uriHash = uri.ToString().GetHashCode();
            if (depth > maxDepth || parsed.GetValueOrDefault(uriHash, false))
            {
                return;
            }

            parsed[uriHash] = true;
            var text = DaemonUtils.ReadTextFromUrl(uri);
            if (text == null)
            {
                return;
            }

            var add = sitesApi.AddSiteFile(siteId, uri.LocalPath, text);
            var uris = UrlRegex.Matches(text)
                .Select(m => m.Groups[1].Captures[0].Value)
                .Select(m => Uri.TryCreate(m, UriKind.Relative, out var u)
                    ? new Uri(uri, u.ToString())
                    : Uri.TryCreate(m, UriKind.Absolute, out var ur) && ur.Host == uri.Host
                        ? ur
                        : null)
                .Where(u => u != null && DaemonUtils.IsWebPage(u.ToString()));
            await add;
            await Task.WhenAll(uris.Select(u => Parse(siteId, u, depth + 1, maxDepth, parsed, token)).ToArray());
        }
    }
}