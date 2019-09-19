using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Xamarin.Essentials;
using XrnCourse.FavoriteSites.Domain.Models;

namespace XrnCourse.FavoriteSites.Domain.Services.FileIO
{
    public class JsonSiteRepository : ISiteRepository
    {
        public IList<Site> GetSites()
        {
            string fullPath = GetFilePath();
            try
            {
                string sitesJson = File.ReadAllText(fullPath);
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
            return AddSite(site);        }

        protected void SaveSitesToFile(IEnumerable<Site> sites)
        {
            string sitesJson = JsonConvert.SerializeObject(sites);
            string fullPath = GetFilePath();
            File.WriteAllText(fullPath, sitesJson);
        }

        protected string GetFilePath()
        {
            return Path.Combine(FileSystem.AppDataDirectory, Constants.FavoriteSitesFileName);
        }
    }
}
