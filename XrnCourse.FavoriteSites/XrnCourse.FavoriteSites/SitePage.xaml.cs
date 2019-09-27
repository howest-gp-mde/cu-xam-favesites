using FluentValidation;
using System;
using System.IO;
using Xamarin.Essentials;
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

            string filePath = Path.Combine(FileSystem.AppDataDirectory, Constants.FavoriteSitesFileName);

            siteRepository = new JsonSiteRepository(filePath);
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
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

            try
            {
                var savedSite = siteService.Save(site);
                Site = savedSite;
                await Navigation.PopAsync();
            }
            catch(ValidationException valEx)
            {
                foreach (var error in valEx.Errors)
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
            catch(Exception ex)
            {
                await DisplayAlert("Error while saving", ex.Message, "Ok");
            }
        }

    }
}