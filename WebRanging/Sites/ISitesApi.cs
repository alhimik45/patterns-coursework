using System.Collections.Generic;
using System.Threading.Tasks;
using WebRanging.Daemons.Analyzer;

namespace WebRanging.Sites
{
    public interface ISitesApi
    {
        Task<string> NewSite(string url);
        Task AddSiteFile(string siteId, string filename, string content);
        IEnumerable<FileInfo> GetSiteFiles(string siteId);
        Task<List<Site>> GetList();
        Task SetSiteParam(string siteId, WebRageType paramName, long value, int weight);
        Task UpdateResultWebmetrick(string siteId, int analyzersWeight);
        Task MarkAnalyzed(string siteId);
        Task<bool> CheckAnalyzed(string siteId);
    }
}