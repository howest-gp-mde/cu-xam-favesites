using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XrnCourse.FavoriteSites.Domain.Models;

namespace XrnCourse.FavoriteSites.Domain.Services
{
    public interface ISiteService
    {
        IEnumerable<Site> GetAll();
        Site Get(Guid siteId);
        (Site, IEnumerable<ValidationFailure>) Save(Site site);
        Site Delete(Guid siteId);
        Task Open(Guid siteId);
    }
}
