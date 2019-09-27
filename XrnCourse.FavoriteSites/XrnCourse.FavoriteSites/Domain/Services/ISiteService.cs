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
        /// <summary>
         /// Gets all Sites, ordered by number of visits, then by name
         /// </summary>
        IEnumerable<Site> GetAll();

        /// <summary>
        /// Gets a Site by its Id
        /// </summary>
        Site Get(Guid siteId);

        /// <summary>
        /// Updates an existing Site using its Id, and returns the updated Site on success
        /// </summary>
        Site Save(Site site);

        /// <summary>
        /// Adds a Site, and returns the added Site on success
        /// </summary>
        Site Delete(Guid siteId);

        /// <summary>
        /// Deletes a Site using its Id and returns the removed Site on success
        /// </summary>
        Task Open(Guid siteId);
    }
}
