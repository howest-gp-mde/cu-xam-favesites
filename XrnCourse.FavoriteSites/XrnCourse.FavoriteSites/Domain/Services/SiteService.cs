using FluentValidation;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
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
            return _siteRepository.GetSites();
        }

        public Site Get(Guid siteId)
        {
            return _siteRepository.GetSite(siteId);
        }

        public Site Delete(Guid siteId)
        {
            return null;
        }

        public Site Save(Site site)
        {
            var results = _siteValidator.Validate(site);
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
            await Browser.OpenAsync(site.Url, BrowserLaunchMode.External);
            site.TimesVisited++;
            Save(site);
        }

    }
}
