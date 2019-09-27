using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using Xamarin.Essentials;
using Xamarin.Forms;
using XrnCourse.FavoriteSites.Domain.Models;
using XrnCourse.FavoriteSites.Domain.Services;
using XrnCourse.FavoriteSites.Domain.Services.FileIO;

namespace XrnCourse.FavoriteSites
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        private ISiteRepository siteRepository;
        private ISiteService siteService;

        public ObservableCollection<Site> Sites { get; } = new ObservableCollection<Site>();

        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = this;

            string filePath = Path.Combine(FileSystem.AppDataDirectory, Constants.FavoriteSitesFileName);

            siteRepository = new JsonSiteRepository(filePath);
            siteService = new SiteService(siteRepository);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            RefreshSites();
        }


        private async void LvFavoriteSites_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            try
            {
                var selectedSite = e.Item as Site;
                await siteService.Open(selectedSite.Id);
            }
            catch(Exception ex)
            {
                await DisplayAlert("Can't open site", ex.Message, "Ok");
            }
        }

        private void LvFavoriteSitesDelete_Clicked(object sender, EventArgs e)
        {
            var selectedSite = ((MenuItem)sender).CommandParameter as Site;
            siteService.Delete(selectedSite.Id);
            RefreshSites();
        }

        private async void LvFavoriteSitesEdit_Clicked(object sender, EventArgs e)
        {
            var selectedSite = ((MenuItem)sender).CommandParameter as Site;
            await Navigation.PushAsync(new SitePage(selectedSite));
        }

        private async void BtnAddSite_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SitePage(null));
        }

        private void RefreshSites()
        {
            var sites = siteService.GetAll();
            Sites.Clear();
            foreach (var site in sites)
                Sites.Add(site);
        }
    }
}
