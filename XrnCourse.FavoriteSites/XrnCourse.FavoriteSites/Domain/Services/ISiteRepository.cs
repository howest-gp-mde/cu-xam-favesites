using System;
using System.Collections.Generic;
using XrnCourse.FavoriteSites.Domain.Models;

namespace XrnCourse.FavoriteSites.Domain.Services
{
    public interface ISiteRepository
    {
        IList<Site> GetSites();

        Site GetSite(Guid id);

        Site UpdateSite(Site site);

        Site AddSite(Site site);

        Site DeleteSite(Guid id);

    }
}
