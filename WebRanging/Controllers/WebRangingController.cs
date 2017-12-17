using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebRanging.Daemons;
using WebRanging.Queue;
using WebRanging.Sites;

namespace WebRanging.Controllers
{
    public class WebRangingController : Controller
    {
        private readonly ISitesApi sitesApi;
        private readonly IDaemonsApi daemonsApi;
        private readonly IQueueApi queueApi;

        public WebRangingController(ISitesApi sitesApi, IDaemonsApi daemonsApi, IQueueApi queueApi)
        {
            this.sitesApi = sitesApi;
            this.daemonsApi = daemonsApi;
            this.queueApi = queueApi;
        }

        [Route("site-list")]
        public async Task<object> SiteList()
        {
            return await sitesApi.GetList();
        }

        [Route("analyse-again/{siteId}")]
        public async Task<object> Analyze([FromRoute] string siteId)
        {
            var sites = await sitesApi.GetList();
            var ss = sites.Single(s => s.Id == siteId);
            await queueApi.Add(new QueueJobBuilder().OfAnalyze(siteId, ss.Url).ForceAnalyze().Build());
            return true;
        }

        [Route("analyse-again")]
        public async Task<object> AnalyzeAll()
        {
            var sites = await sitesApi.GetList();
            foreach (var site in sites)
            {
                await queueApi.Add(new QueueJobBuilder().OfAnalyze(site.Id, site.Url).ForceAnalyze().Build());
            }

            return true;
        }

        [Route("get-queues")]
        public async Task<object> GetQueues()
        {
            return new
            {
                Parse = await queueApi.GetList(JobType.ParseSite),
                Analyze = await queueApi.GetList(JobType.AnalyzeSite)
            };
        }

        [Route("new-sites")]
        public async Task<object> NewSites([FromForm] string siteList)
        {
            var queries = siteList.Split("\n")
                .Select(l =>
                {
                    uint n = 5;
                    var p = l.Split(",");
                    if (p.Length > 1)
                    {
                        if (!uint.TryParse(p[1].Trim(), out n))
                        {
                            n = 5;
                        }
                    }

                    return new
                    {
                        Url = p[0].Trim(),
                        Depth = n
                    };
                }).Select(q =>
                    queueApi.Add(new QueueJobBuilder().OfParsing(q.Url).ParsingDepth(q.Depth).Build()))
                .ToArray();
            await Task.WhenAll(queries);
            return Redirect("/queue.html");
        }

        [Route("get-daemons")]
        public async Task<object> GetDaemons()
        {
            return new
            {
                Parse = daemonsApi.GetList(DaemonType.Parser),
                Analyze = daemonsApi.GetList(DaemonType.Analyzer)
            };
        }

        [Route("run-d/{type}")]
        public async Task<object> RunDaemon([FromRoute] DaemonType type)
        {
            daemonsApi.Run(type, 1);
            return true;
        }

        [Route("stop-d/{type}")]
        public async Task<object> StopDaemon([FromRoute] DaemonType type)
        {
            await daemonsApi.Stop(type, 1);
            return true;
        }

        [Route("stop-name/{id}")]
        public async Task<object> StopDaemonById([FromRoute] Guid id)
        {
            await daemonsApi.Stop(id);
            return true;
        }
    }
}