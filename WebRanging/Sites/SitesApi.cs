using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using Newtonsoft.Json;
using WebRanging.Daemons.Analyzer;

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

        public async Task<IEnumerable<FileInfo>> GetSiteFiles(string siteId)
        {
            var s = (await sites.FindAsync(site => site.Id == siteId)).First();
            var siteDir = Path.Combine(configProvider.StoreSitesFolder, siteId);
            return Directory.EnumerateFiles(siteDir)
                .Select(path => new FileInfo
                {
                    OwnerHost = s.Url,
                    Filename = Uri.UnescapeDataString(path.Replace(siteDir, "")),
                    Content = new Lazy<string>(() => File.ReadAllText(path))
                });
        }

        public async Task<List<Site>> GetList()
        {
            var s = await sites.FindAsync(_ => true,
                new FindOptions<Site>
                {
                    Sort = new SortDefinitionBuilder<Site>()
                        .Descending(site => site.ResultWebMetrick)
                });
            return s.ToList();
        }

        public async Task SetSiteParam(string siteId, WebRageType paramName, long value, int weight)
        {
            await sites.UpdateOneAsync(
                ss => ss.Id == siteId,
                new UpdateDefinitionBuilder<Site>()
                    .Set($"Params.{JsonConvert.SerializeObject(paramName).Trim('"')}", value)
                    .Set($"WeightedParams.{JsonConvert.SerializeObject(paramName).Trim('"')}", value*weight));
        }

        public async Task UpdateResultWebmetrick(string siteId, int analyzersWeight)
        {
            var s = (await sites.FindAsync(site => site.Id == siteId)).First();
            var result = (float)s.WeightedParams.Values.Sum() / analyzersWeight;
            await sites.UpdateOneAsync(
                ss => ss.Id == siteId,
                new UpdateDefinitionBuilder<Site>()
                    .Set(ss => ss.ResultWebMetrick, result));
        }

        public async Task MarkAnalyzed(string siteId)
        {
            var s = (await sites.FindAsync(site => site.Id == siteId)).First();
            await sites.UpdateOneAsync(
                ss => ss.Id == siteId,
                new UpdateDefinitionBuilder<Site>()
                    .Set(ss => ss.AnalyzedBundle, s.BundleVersion));
        }

        public async Task<bool> CheckAnalyzed(string siteId)
        {
            var s = (await sites.FindAsync(site => site.Id == siteId)).First();
            return s.AnalyzedBundle == s.BundleVersion;
        }
    }
}