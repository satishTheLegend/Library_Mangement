using Library_Mangement.Views.FlyoutView.FlyoutSubViews;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Views.FlyoutView
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DashboardFlyoutViewFlyout : ContentPage
    {
        public ListView ListView;

        public DashboardFlyoutViewFlyout()
        {
            InitializeComponent();

            BindingContext = new DashboardFlyoutViewFlyoutViewModel();
            if(App.CurrentLoggedInUser != null)
            {
                UserName.Text = $"{App.CurrentLoggedInUser.FirstName} {App.CurrentLoggedInUser.LastName}";
                UserPhone.Text = $"+91 {App.CurrentLoggedInUser.Phone}";
                UserPic.ImageSource = App.CurrentLoggedInUser != null ? App.CurrentLoggedInUser.ProfilePicPath : "user.png";

            }
            ListView = MenuItemsListView;
        }

        private class DashboardFlyoutViewFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<DashboardFlyoutViewFlyoutMenuItem> MenuItems { get; set; }

            public DashboardFlyoutViewFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<DashboardFlyoutViewFlyoutMenuItem>(new[]
                {
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 1, Title = "Home", Icon = "home.png", TargetType = typeof(DashboardFlyoutView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 2, Title = "Profile", Icon = "user.png", TargetType = typeof(ProfileView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 3, Title = "Explore Books", Icon = "explore_books.png", TargetType = typeof(CategoryView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 4, Title = "Notifications", Icon = "notification.png", TargetType = typeof(MyNotificationView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 5, Title = "Cart", Icon = "shopping_cart.png", TargetType = typeof(MyCartView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 6, Title = "My Books", Icon = "order.png", TargetType = typeof(MyBooksView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 7, Title = "Wish List", Icon = "WishList.png", TargetType = typeof(WhishListView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 8, Title = "Account Settings", Icon = "accsettings.png", TargetType = typeof(AccountSettingsView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 9, Title = "My Reviews", Icon = "rating.png", TargetType = typeof(MyReviewsView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 10, Title = "App Settings", Icon = "AppSettings.png", TargetType = typeof(AppSettingsView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 11, Title = "Help Support", Icon = "support.png", TargetType = typeof(HelpAndSupportView)},
                    new DashboardFlyoutViewFlyoutMenuItem{  Id = 12, Title = "Logout", Icon = "logout.png", TargetType = typeof(UserLogin)},
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}