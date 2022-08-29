using Library_Mangement.Views.FlyoutView.FlyoutSubViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Views.FlyoutView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardFlyoutViewDetail : ContentPage
    {
        public DashboardFlyoutViewDetail()
        {
            InitializeComponent();
            profileView.ImageSource = App.CurrentLoggedInUser != null ? App.CurrentLoggedInUser.ProfilePicPath : "user.png";
            UserData.Text = App.CurrentLoggedInUser != null ? $"Welcome Back, {App.CurrentLoggedInUser.FirstName} {App.CurrentLoggedInUser.LastName}" : "Welcome Back, John Doe";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (Application.Current.MainPage.Navigation.NavigationStack != null && Application.Current.MainPage.Navigation.NavigationStack.Count > 1)
            {
                var existingPages = Application.Current.MainPage.Navigation.NavigationStack.ToList();
                foreach (var pageItem in existingPages)
                {
                    bool flag = (pageItem is DashboardFlyoutViewDetail) || (pageItem is DashboardFlyoutView);
                    if (!flag)
                    {
                        Application.Current.MainPage.Navigation.RemovePage(pageItem);
                    }
                }
            }
        }

        private async void ExploreBooksCommand(object sender, EventArgs e)
        {
            await App.Current.MainPage.Navigation.PushAsync(new CategoryView());
        }
    }
}