using Library_Mangement.Database;
using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Resx;
using Library_Mangement.Services;
using Library_Mangement.Themes;
using Library_Mangement.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement
{
    public partial class App : Application
    {
        #region Properties
        public static LogDatabase LogDatabase;
        public static AppDatabase Database;
        public static RestService RestServiceConnection;
        public static tblUser CurrentLoggedInUser { get; set; }
        public static bool IsAppInitialize = false;
        #endregion

        #region Constructor
        public App()
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("NjAzMjM4QDMxMzkyZTM0MmUzMEZxVkRMK3NlSk0zamgxYi9NMFdnOURXRjFrci9IVkRNTDZ1M2ljT1BvY2M9");
            InitializeComponent();
            RestServiceConnection = new RestService();
            DevExpress.XamarinForms.Editors.Initializer.Init();
            ThemeManager.ChangeTheme(AppConfig.AppTheme_Theme);
            LocalDatabase.Init();
            if (CurrentLoggedInUser == null)
            {
                CurrentLoggedInUser = App.Database.User.GetActiveUserData();
            }
            RedirectToMainScreen();
        }
        #endregion

        #region Private Methods
        private async void RedirectToMainScreen()
        {
            try
            {
                if (CurrentLoggedInUser != null)
                {
                    bool sessionExpire = false;
                    var logInTime = App.CurrentLoggedInUser.LoginTime;

                    var sessionLogoutTime = logInTime.AddMinutes(AppConfig.SessionTimeOut_Minutes);
                    int resTimeDifference = DateTime.Compare(DateTime.Now, sessionLogoutTime);
                    sessionExpire = resTimeDifference > 0;

                    if (sessionExpire)
                    {
                        MainPage = new NavigationPage(new LandingView(true));
                        await App.Current.MainPage.DisplayAlert(AppResources.Session_TimeOut_Alert_Title, AppResources.Session_TimeOut_Msg, AppResources.Ok);
                    }
                    else
                    {
                        //MainPage = new NavigationPage(new FlyoutView(false, true));
                    }
                }
                else
                {
                    MainPage = new NavigationPage(new SpalshView());
                    //MainPage = new NavigationPage(new CatagoryChips());
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Override Methods
        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        } 
        #endregion
    }
}
