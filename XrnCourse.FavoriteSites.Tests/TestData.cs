using System;
using XrnCourse.FavoriteSites.Domain.Models;

namespace XrnCourse.FavoriteSites.Tests
{
    class TestData
    {
        public static Site[] TestSites => new [] {
            new Site
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = "Site",
                Rating = 5,
                TimesVisited = 1,
                Url = "https://www.github.com"
            },
            new Site
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = "Site",
                Rating = 3,
                TimesVisited = 2,
                Url = "https://www.howest.be"
            },
            new Site
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = "Another Site",
                Rating = 3,
                TimesVisited = 2,
                Url = "https://www.google.com"
            },
        };

        public static Site[] InvalidPrefixedSites => new[] {
            new Site
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = "Site",
                Rating = 5,
                TimesVisited = 1,
                Url = "www.github.com"  // this is the url without prefix
            }
        };
    }
}
