using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WebRanging.Queue;
using WebRanging.Sites;

namespace WebRanging.Daemons.Parser
{
    public class ParserDaemon : TaskDaemon
    {
        private readonly IQueueApi queueApi;
        private readonly ISitesApi sitesApi;
        public override DaemonType Type { get; } = DaemonType.Parser;

        private static readonly Regex urlRegex = new Regex(@"href=""([^""]+)""");

        public ParserDaemon(IQueueApi queueApi, ISitesApi sitesApi)
        {
            this.queueApi = queueApi;
            this.sitesApi = sitesApi;
        }

        protected override async Task ExecuteIteration(CancellationToken token)
        {
            try
            {
                var task = await queueApi.Fetch(JobType.ParseSite);
                while (task == null)
                {
                    await Task.Delay(1000);
                    task = await queueApi.Fetch(JobType.ParseSite);
                }

                var url = task.Arguments["url"];
                if (!ParserUtils.IsUrl(url))
                {
                    return;
                }

                var depthStr = task.Arguments.GetValueOrDefault("parseDepth", "5");
                if (!int.TryParse(depthStr, out var depth))
                {
                    depth = 5;
                }

                var uri = new Uri(url);
                var siteId = await sitesApi.NewSite(uri.Host);
                status = $"Скачивание {uri.Host}";
                await Parse(siteId, uri, 0, depth, token);
                await queueApi.Add(new QueueJobBuilder().OfAnalyze(siteId, uri.Host).Build());
                status = DaemonConstants.IdleStatus;
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        private async Task Parse(string siteId, Uri uri, int depth, int maxDepth, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            if (depth > maxDepth)
            {
                return;
            }

            var text = ParserUtils.ReadTextFromUrl(uri);
            if (text == null)
            {
                return;
            }

            var add = sitesApi.AddSiteFile(siteId, uri.LocalPath, text);
            var uris = urlRegex.Matches(text)
                .Select(m => m.Groups[1].Captures[0].Value)
                .Select(m => Uri.TryCreate(m, UriKind.Relative, out var u) ? u : null)
                .Where(u => u != null)
                .Select(u => new Uri(uri, u.ToString()));
            await add;
            Parallel.ForEach(uris,
                new ParallelOptions
                {
                    CancellationToken = token
                },
                async u => await Parse(siteId, u, depth + 1, maxDepth, token));
        }
    }
}