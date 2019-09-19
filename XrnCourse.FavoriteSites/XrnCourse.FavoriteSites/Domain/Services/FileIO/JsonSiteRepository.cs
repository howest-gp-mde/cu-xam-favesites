using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using XrnCourse.FavoriteSites.Domain.Models;

namespace XrnCourse.FavoriteSites.Domain.Services.FileIO
{
    public class JsonSiteRepository : ISiteRepository
    {
        private readonly string _filePath;

        public JsonSiteRepository(string filePath)
        {
            _filePath = filePath;
        }

        public IList<Site> GetSites()
        {
            try
            {
                string sitesJson = File.ReadAllText(_filePath);
                var sites = JsonConvert.DeserializeObject<IEnumerable<Site>>(sitesJson);
                return sites.ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error reading sites: {ex.Message}");
                return new List<Site>();
            }
        }

        public Site AddSite(Site site)
        {
            var sites = GetSites();
            sites.Add(site);

            SaveSitesToFile(sites);

            return GetSite(site.Id);
        }

        public Site DeleteSite(Guid id)
        {
            var sites = GetSites();
            var siteToRemove = sites.FirstOrDefault(e => e.Id == id);
            sites.Remove(siteToRemove);

            SaveSitesToFile(sites);

            return siteToRemove;
        }

        public Site GetSite(Guid id)
        {
            var sites = GetSites();
            return sites.FirstOrDefault(e => e.Id == id);
        }

        public Site UpdateSite(Site site)
        {
            DeleteSite(site.Id);
            return AddSite(site);
        }

        protected void SaveSitesToFile(IEnumerable<Site> sites)
        {
            string sitesJson = JsonConvert.SerializeObject(sites);
            File.WriteAllText(_filePath, sitesJson);
        }

    }
}
