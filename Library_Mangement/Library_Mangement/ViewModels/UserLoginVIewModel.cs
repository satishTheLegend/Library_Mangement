using Library_Mangement.Validation;
using Library_Mangement.Views;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class UserLoginVIewModel : ValidatableBase
    {
        #region Properties

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
                if (UserName == "Arpita" && Password == "123")
                {
                    await App.Current.MainPage.DisplayAlert("", "You Logged In Successfully", "Ok");
                    await App.Current.MainPage.Navigation.PushAsync(new CatagoryChips());
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", "You Have Entered Wrong Credentials", "Ok");
                }
            }
            await Task.FromResult(Task.CompletedTask);
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
