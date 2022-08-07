using Library_Mangement.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [DesignTimeVisible(false)]
    public partial class Dashboard : ContentPage
    {
        #region Properties
        readonly DashboardViewModel _vm;
        #endregion

        #region Constructor
        public Dashboard()
        {
            try
            {
                InitializeComponent();
                //_vm = new DashboardViewModel();
                //BindingContext = _vm;
                MyMenu = GetMenus();
                this.BindingContext = this;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Override Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //await _vm.LoadBooksInfo_Updated();
        }
        #endregion

        #region Public Methods
        public List<Menu> MyMenu { get; set; }

        private List<Menu> GetMenus()
        {
            return new List<Menu>
            {
                new Menu{ Name = "Home", Icon = "home.png"},
                new Menu{ Name = "Profile", Icon = "user.png"},
                new Menu{ Name = "Notifications", Icon = "notification.png"},
                new Menu{ Name = "Cart", Icon = "shopping_cart.png"},
                new Menu{ Name = "My Orders", Icon = "order.png"},
                new Menu{ Name = "Wish List", Icon = "WishList.png"},
                new Menu{ Name = "Account Settings", Icon = "accsettings.png"},
                new Menu{ Name = "My Reviews", Icon = "rating.png"},
                new Menu{ Name = "App Settings", Icon = "AppSettings.png"},
                new Menu{ Name = "Help Support", Icon = "support.png"},
                new Menu{ Name = "Logout", Icon = "logout.png"},
            };
        }
        #endregion

        #region Private Methods
        private async void OpenAnimation()
        {
            await swipeContent.ScaleYTo(0.9, 300, Easing.SinOut);
            pancake.CornerRadius = 20;
            await swipeContent.RotateTo(-15, 300, Easing.SinOut);
        }

        private async void CloseAnimation()
        {
            await swipeContent.RotateTo(0, 300, Easing.SinOut);
            pancake.CornerRadius = 0;
            await swipeContent.ScaleYTo(1, 300, Easing.SinOut);
        }

        private void OpenSwipe(object sender, EventArgs e)
        {
            MainSwipeView.Open(OpenSwipeItem.LeftItems);
            OpenAnimation();
        }

        private void CloseSwipe(object sender, EventArgs e)
        {
            MainSwipeView.Close();
            CloseAnimation();
        }

        private void SwipeStarted(object sender, SwipeStartedEventArgs e)
        {
            OpenAnimation();
        }

        private void SwipeEnded(object sender, SwipeEndedEventArgs e)
        {
            if (!e.IsOpen)
                CloseAnimation();
        }

        //private void CloseRequested(object sender, EventArgs e)
        //{
        //    CloseAnimation();
        //}
        #endregion
    }
    public class Menu
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }

}