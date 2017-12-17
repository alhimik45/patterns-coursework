using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace WebRanging.Sites
{
    public class SitesApi : ISitesApi
    {
        private readonly IConfigProvider configProvider;
        private IMongoCollection<Site> sites;

        public SitesApi(IDbContext db, IConfigProvider configProvider)
        {
            this.configProvider = configProvider;
            sites = db.Sites;
        }

        public async Task<string> NewSite(string url)
        {
            var s = (await sites.FindAsync(ss => ss.Url == url)).ToList();
            if (s.Count > 0)
            {
                return s.First().Id;
            }

            var site = new Site
            {
                Url = url,
                BundleVersion = Guid.NewGuid()
            };
            await sites.InsertOneAsync(site);

            var sitesDir = configProvider.StoreSitesFolder;
            if (!Directory.Exists(sitesDir))
            {
                Directory.CreateDirectory(sitesDir);
            }

            var siteDir = Path.Combine(sitesDir, site.Id);
            if (!Directory.Exists(siteDir))
            {
                Directory.CreateDirectory(siteDir);
            }

            return site.Id;
        }

        public async Task AddSiteFile(string siteId, string filename, string content)
        {
            var result = await sites.UpdateOneAsync(
                site => site.Id == siteId,
                new UpdateDefinitionBuilder<Site>()
                    .Set(site => site.BundleVersion, Guid.NewGuid()));
            if (result.ModifiedCount != 1)
            {
                throw new InvalidOperationException("Trying to change non-existent site");
            }

            var siteDir = Path.Combine(configProvider.StoreSitesFolder, siteId);
            File.WriteAllText(Path.Combine(siteDir, Uri.EscapeDataString(filename)), content);
        }

        public IEnumerable<string> GetSiteFiles(string siteId)
        {
            var siteDir = Path.Combine(configProvider.StoreSitesFolder, siteId);
            return Directory.EnumerateFiles(siteDir)
                .Select(File.ReadAllText);
        }

        public async Task<List<Site>> GetList()
        {
            var s = await sites.FindAsync(_ => true);
            return s.ToList();
        }
    }
}