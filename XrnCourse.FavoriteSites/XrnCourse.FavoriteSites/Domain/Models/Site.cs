using System;

namespace XrnCourse.FavoriteSites.Domain.Models
{
    public class Site
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public double Rating { get; set; }
        public long TimesVisited { get; set; }
    }
}
