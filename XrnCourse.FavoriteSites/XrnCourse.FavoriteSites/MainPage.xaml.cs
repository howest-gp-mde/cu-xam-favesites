using System;
using System.ComponentModel;
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

        public MainPage()
        {
            InitializeComponent();

            siteRepository = new JsonSiteRepository();
            siteService = new SiteService(siteRepository);
        }

        private void ContentPage_Appearing(object sender, EventArgs e)
        {
            RefreshSites();
        }

        private async void LvFavoriteSites_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var selectedSite = e.Item as Site;
            await siteService.Open(selectedSite.Id);
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
            lvFavoriteSites.ItemsSource = sites;
        }
    }
}
