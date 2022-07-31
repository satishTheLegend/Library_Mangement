using Library_Mangement.Model;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class UserRegistrationViewModel : ValidatableBase
    {
        #region Properties
        private UserRegistrationModel _registration;
        public UserRegistrationModel Registration
        {
            get
            {
                return _registration;
            }
            set
            {
                _registration = value;
                OnPropertyChanged(nameof(Registration));
            }
        }
        #endregion

        #region Constructor

        #endregion

        #region Commands
        public ICommand RegisterUserCommand => new Command(async ()=> await RegisterUser());

        #endregion

        #region Event Handlers
        private Task RegisterUser()
        {
            
        }
        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}
