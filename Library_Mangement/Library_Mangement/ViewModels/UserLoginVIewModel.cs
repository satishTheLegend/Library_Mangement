using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model.ApiResponse.GETModels;
using Library_Mangement.Resx;
using Library_Mangement.Services;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class UserLoginVIewModel : ValidatableBase
    {
        #region Properties
        private readonly string _moduleName = nameof(UserLoginVIewModel);
        private string _userName;
        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                OnPropertyChanged(nameof(UserName));
            }
        }
        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        #endregion

        #region Constructor
        public UserLoginVIewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand LoginButtonCommand => new Command(async () => await Login());
        #endregion

        #region Event Handlers
        private async Task Login()
        {
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                var resp = await RestService.LoginAsync(UserName, Password);
                var loginData = resp.Result.data;
                if (loginData != null)
                {
                    await SaveUserLogin(loginData);
                    await App.Current.MainPage.DisplayAlert("", $"Welcome Back {loginData.firstName} {loginData.lastName}", AppResources.Ok);
                    if(!string.IsNullOrEmpty(loginData.catagories))
                        await App.Current.MainPage.Navigation.PushAsync(new CatagoryChips());
                    else
                        await App.Current.MainPage.Navigation.PushAsync(new HomeView());
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", $"Sorry We Count Found Any Account Against to this Email {UserName}", AppResources.Ok);
                }
            }
            await Task.FromResult(Task.CompletedTask);
        }


        #endregion

        #region Public Methods

        #endregion

        #region Private Methods
        private async Task SaveUserLogin(LoginData loginData)
        {
            try
            {
                string profileDirectoryPath = Common.GetBasePath(AppConfig.DirName_Profile_Pic);
                string profilePath = !string.IsNullOrEmpty(loginData.profileAvatar) ? await RestService.DownloadFile(loginData.profileAvatar, profileDirectoryPath) : string.Empty;
                tblUser user = new tblUser()
                {
                    FirstName = loginData.firstName,
                    LastName = loginData.lastName,
                    Email = loginData.email,
                    DOB = loginData.birthDate,
                    Catagories = loginData.catagories,
                    CollageName = loginData.collageName,
                    Education = loginData.currentEducation,
                    Phone = loginData.phone,
                    RollNo = loginData.rollNo,
                    UserName = loginData.userName,
                    UserToken = loginData.userToken,
                    LoginTime = DateTime.Now,
                    Gender = loginData.gender,
                    ProfilePicPath = profilePath,
                    ProfilePicUrl = loginData.profileAvatar
                };
                App.CurrentLoggedInUser = user;
                await App.Database.User.InsertAsync(user);
                App.LogDatabase.Log.AddLogs(AppConfig.LogType_Info, _moduleName, $"User Login Data :::: {user}");
            }
            catch (Exception ex)
            {
                ExceptionHandlerService.SendErrorLog(_moduleName, ex);
            }
        }
        #endregion
    }
}
