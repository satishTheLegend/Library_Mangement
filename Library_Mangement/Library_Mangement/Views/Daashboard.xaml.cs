using Acr.UserDialogs;
using Library_Mangement.Resx;
using Library_Mangement.Services;
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
        bool _isOpened = false;
        #endregion

        #region Constructor
        public Dashboard()
        {
            try
            {
                InitializeComponent();
                _vm = new DashboardViewModel();
                BindingContext = _vm;
                //MyMenu = GetMenus();
                //this.BindingContext = this;
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
            _isOpened = false;
            await swipeContent.RotateTo(0, 300, Easing.SinOut);
            pancake.CornerRadius = 0;
            await swipeContent.ScaleYTo(1, 300, Easing.SinOut);
        }

        private void OpenSwipe(object sender, EventArgs e)
        {
            if (!_isOpened)
            {
                MainSwipeView.Open(OpenSwipeItem.LeftItems);
                OpenAnimation();
                _isOpened = true;
            }
            else
            {
                MainSwipeView.Close();
                CloseAnimation();
            }
        }

        private async void CloseSwipe(object sender, EventArgs e)
        {
            var stack = sender as StackLayout;
            if (stack != null)
            {
                var menuItem = stack.BindingContext as ViewModels.Menu;
                if (menuItem != null)
                {
                    switch (menuItem.Name)
                    {
                        case "Home":
                            _vm.dashboard.DashboardVisible = true;
                            _vm.profile.ProfileVisible = false;
                            break;

                        case "Profile":
                            if (_vm.profile.FieldItems == null || _vm.profile.FieldItems?.Count < 1)
                                await _vm.LoadRegistrationForm(profileStack);
                            else
                                _vm.ShowForm();
                            break;

                        case "Explore Books":
                            await App.Current.MainPage.Navigation.PushAsync(new HomeView());//_vm.ExploreBooksClicked();
                            //BooksUICards.ParentBindingContext = BindingContext;
                            break;

                        case "Notifications":
                            break;

                        case "Cart":
                            break;

                        case "My Orders":
                            break;

                        case "Wish List":

                            break;

                        case "Account Settings":
                            break;

                        case "My Reviews":
                            break;

                        case "App Settings":

                            break;

                        case "Help Support":

                            break;

                        case "Logout":

                            break;

                        default:
                            break;
                    }
                }
            }
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
            {
                MainSwipeView.Close();
                CloseAnimation();
            }
        }

        //private void CloseRequested(object sender, EventArgs e)
        //{
        //    CloseAnimation();
        //}

        private void SearchBar_Focused(object sender, FocusEventArgs e)
        {
            //overviewstack.IsVisible = false;
        }

        private void SearchBar_Unfocused(object sender, FocusEventArgs e)
        {
            //overviewstack.IsVisible = true;
        }

        #endregion

        private async void OpenProfile(object sender, EventArgs e)
        {
            UserDialogs.Instance.ShowLoading();
            if (_vm.profile.FieldItems == null || _vm.profile.FieldItems?.Count < 1)
                await _vm.LoadRegistrationForm(profileStack);
            else
                _vm.ShowForm();
            await Task.Delay(200);
            UserDialogs.Instance.HideLoading();
        }

    }


}