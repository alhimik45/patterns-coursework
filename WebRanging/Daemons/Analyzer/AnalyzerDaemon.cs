using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebRanging.Queue;
using WebRanging.Sites;

namespace WebRanging.Daemons.Analyzer
{
    public class AnalyzerDaemon : TaskDaemon
    {
        private readonly IQueueApi queueApi;
        private readonly ISitesApi sitesApi;
        private readonly List<IAnalyzerProvider> analyzerProviders;
        public override DaemonType Type { get; } = DaemonType.Analyzer;

        public AnalyzerDaemon(IQueueApi queueApi, ISitesApi sitesApi, List<IAnalyzerProvider> analyzerProviders)
        {
            this.queueApi = queueApi;
            this.sitesApi = sitesApi;
            this.analyzerProviders = analyzerProviders;
        }

        protected override async Task ExecuteIteration(CancellationToken token)
        {
            try
            {
                var task = await queueApi.Fetch(JobType.AnalyzeSite);
                while (task == null)
                {
                    await Task.Delay(1000, token);
                    task = await queueApi.Fetch(JobType.AnalyzeSite);
                }

                var siteId = task.Arguments["site"];
                if (await sitesApi.CheckAnalyzed(siteId) && !task.Arguments.ContainsKey("forceAnalyze"))
                {
                    return;
                }

                var url = task.Arguments["url"];
                status = $"Анализ {url}";

                var files = sitesApi.GetSiteFiles(siteId);
                try
                {
                    var analyzerChain = CreateAnalyzerChain();
                    foreach (var file in files)
                    {
                        status = $"Анализ {url}{file.Filename}";
                        analyzerChain.ProcessFile(file.Content, token);
                    }

                    analyzerChain.SaveWebometricsValue(
                        async (type, val, weight) => await sitesApi.SetSiteParam(siteId, type, val, weight));
                    await sitesApi.UpdateResultWebmetrick(siteId, analyzerChain.ChainWeight);
                }
                catch (OperationCanceledException)
                {
                    await queueApi.Add(new QueueJobBuilder().OfAnalyze(siteId, url).ForceAnalyze().Build());
                    throw;
                }

                await sitesApi.MarkAnalyzed(siteId);

                status = DaemonConstants.IdleStatus;
            }
            catch (Exception e) when (!(e is OperationCanceledException))
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
            }
        }

        private IWebometricsAnalyzer CreateAnalyzerChain()
        {
            return analyzerProviders.Aggregate<IAnalyzerProvider, IWebometricsAnalyzer>(
                null,
                (analyzer, provider) => provider.New(analyzer));
        }
    }
}