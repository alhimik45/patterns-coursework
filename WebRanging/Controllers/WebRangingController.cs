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
    }
}