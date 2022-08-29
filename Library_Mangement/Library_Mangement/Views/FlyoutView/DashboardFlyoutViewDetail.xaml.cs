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
    }
}