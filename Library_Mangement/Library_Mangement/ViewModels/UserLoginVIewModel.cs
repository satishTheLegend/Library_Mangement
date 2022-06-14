using Library_Mangement.Validation;
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

        #region CommandsS
        public ICommand LoginButtonCommand => new Command(async () => await Login());

        private async Task Login()
        {
            if(!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                if(UserName == "Arpita" && Password == "123")
                {
                    await App.Current.MainPage.DisplayAlert("", "You Logged In Successfully", "Ok");
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("", "You Have Entered Wrong Credentials", "Ok");
                }
            }
            await Task.FromResult(Task.CompletedTask);
        }
        #endregion

        #region Event Handlers

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
