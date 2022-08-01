using Library_Mangement.Controls;
using Library_Mangement.Helper;
using Library_Mangement.Model;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private StackLayout _mainStack = null;
        private ObservableCollection<DynamicPropertyDataViewModel> _fieldItems;
        public ObservableCollection<DynamicPropertyDataViewModel> FieldItems
        {
            get => _fieldItems;
            set
            {
                _fieldItems = value;
                OnPropertyChanged(nameof(FieldItems));
            }
        }

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
        public UserRegistrationViewModel(StackLayout mainStack)
        {
            _mainStack = mainStack;
        }
        #endregion

        #region Commands
        public ICommand RegisterUserCommand => new Command(async ()=> await RegisterUser());

        #endregion

        #region Event Handlers
        private async Task RegisterUser()
        {
            ValidateModel();
            await Task.FromResult(true);
        }
        #endregion

        #region Public Methods
        public async Task LoadData()
        {
            FieldItems = new ObservableCollection<DynamicPropertyDataViewModel>();
            try
            {
                var RegisterFields = await App.Database.LibraryDynamicFields.GetFieldsByPageName("Registration");
                foreach (var fieldItems in RegisterFields)
                {
                    DynamicPropertyDataViewModel dynamicProperty = new DynamicPropertyDataViewModel();
                    dynamicProperty.FieldId = fieldItems.FieldId;
                    dynamicProperty.PlaceHolderName = fieldItems.FieldName;
                    dynamicProperty.Sequence = fieldItems.Sequence;
                    dynamicProperty.Required = fieldItems.Required;
                    dynamicProperty.FieldName = fieldItems.FieldName;
                    dynamicProperty.ControlType = fieldItems.ControlType;
                    dynamicProperty.PageName = fieldItems.PageName;
                    dynamicProperty.Validation = fieldItems.Validation;
                    dynamicProperty.ValidationMsg = fieldItems.ValidationMsg;
                    dynamicProperty.ListValues = fieldItems.ListValues;

                    FieldItems.Add(dynamicProperty);
                }
                DynamicControlsView uiLoader = new DynamicControlsView();
                StackLayout viewStack = new StackLayout { Padding = new Thickness(5) };
                await uiLoader.LoadView(viewStack, FieldItems);
                if(_mainStack == null)
                {
                    _mainStack = new StackLayout { Padding = new Thickness(5) };
                    _mainStack.Children.Add(viewStack);
                }

            }
            catch(Exception ex)
            {

            }
        }
        #endregion

        #region Private Methods
        private void ValidateModel()
        {
            foreach (var FieldItem in FieldItems)
            {
                if (FieldItem.Required)
                {
                    FieldItem.IsFieldValidationFailed = Common.ValidateInputField(FieldItem);
                }

            }
        }
        #endregion
    }
}
