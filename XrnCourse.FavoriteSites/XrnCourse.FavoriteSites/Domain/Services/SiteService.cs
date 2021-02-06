using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using XrnCourse.FavoriteSites.Domain.Models;
using XrnCourse.FavoriteSites.Domain.Validators;

namespace XrnCourse.FavoriteSites.Domain.Services
{
    public class SiteService : ISiteService
    {
        private ISiteRepository _siteRepository;
        private IValidator _siteValidator;

        public SiteService(ISiteRepository siteRepository)
        {
            _siteRepository = siteRepository;
            _siteValidator = new SiteValidator();
        }

        public IEnumerable<Site> GetAll()
        {
            return _siteRepository.GetSites()
                .OrderByDescending(e => e.TimesVisited)
                .ThenBy(e => e.Name);
        }

        public Site Get(Guid siteId)
        {
            return _siteRepository.GetSite(siteId);
        }

        public Site Delete(Guid siteId)
        {
            return _siteRepository.DeleteSite(siteId);
        }

        public Site Save(Site site)
        {
            site.Url = PrefixUrl(site.Url);

            var validationContext = new ValidationContext<Site>(site);
            var results = _siteValidator.Validate(validationContext);
            var errors = results.Errors;
            if (results.IsValid)
            {
                var existingSite = _siteRepository.GetSite(site.Id);
                if (existingSite != null)
                {
                    var savedSite = _siteRepository.UpdateSite(site);
                    return savedSite;
                }
                else
                {
                    var savedSite = _siteRepository.AddSite(site);
                    return savedSite;
                }
            }
            else
            {
                throw new ValidationException(results.Errors);
            }
        }

        public async Task Open(Guid siteId)
        {
            Site site = Get(siteId);
            if (DeviceInfo.Platform != DevicePlatform.Unknown)
                await Browser.OpenAsync(site.Url, BrowserLaunchMode.External);
            site.TimesVisited++;
            Save(site);
        }

        /// <summary>
        /// Adds https:// to any valid url which only misses a scheme
        /// </summary>
        private string PrefixUrl(string url)
        {
            try
            {
                Uri uri = new Uri(url, UriKind.Absolute);
                if (uri.Scheme == null)
                    url = "https://" + url;
            }
            catch
            {
                url = "https://" + url;
            }
            return url;
        }

    }
}
