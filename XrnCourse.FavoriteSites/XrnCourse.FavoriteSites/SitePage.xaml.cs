using FluentValidation;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XrnCourse.FavoriteSites.Domain.Models;
using XrnCourse.FavoriteSites.Domain.Services;
using XrnCourse.FavoriteSites.Domain.Services.FileIO;

namespace XrnCourse.FavoriteSites
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SitePage : ContentPage
    {
        private ISiteRepository siteRepository;
        private ISiteService siteService;

        public SitePage(Site site)
        {
            InitializeComponent();

            siteRepository = new JsonSiteRepository();
            siteService = new SiteService(siteRepository);

            if (site == null)
            {
                EditMode = false;
                Site = new Site();
            }
            else
            {
                EditMode = true;
                Site = site;
            }
        }

        /// <summary>
        /// Returns the site instance being edited or added
        /// </summary>
        public Site Site { get; private set; }

        /// <summary>
        /// Returns true when editing a site, false when adding a new site
        /// </summary>
        public bool EditMode { get; private set; }


        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            name.Text = Site.Name;
            url.Text = Site.Url;
            rating.Value = Site.Rating;
        }

        private async void BtnSave_Clicked(object sender, EventArgs e)
        {
            Site site = new Site
            {
                Id = Site.Id,
                Name = name.Text,
                Url = url.Text,
                Rating = rating.Value
            };

            if (!EditMode)
                site.Id = Guid.NewGuid();

            var (savedSite, errors) = siteService.Save(site);

            if(errors?.Count() > 0)
            {
                foreach (var error in errors)
                {
                    if (error.PropertyName == nameof(site.Name))
                    {
                        nameError.Text = error.ErrorMessage;
                        nameError.IsVisible = true;
                    }
                    if (error.PropertyName == nameof(site.Url))
                    {
                        urlError.Text = error.ErrorMessage;
                        urlError.IsVisible = true;
                    }
                }
            }
            else
            {
                Site = savedSite;
                await Navigation.PopAsync();
            }
        }

    }
}