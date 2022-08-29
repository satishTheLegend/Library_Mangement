using Acr.UserDialogs;
using Library_Mangement.Controls;
using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model.ApiResponse;
using Library_Mangement.Model.ApiResponse.PostModels;
using Library_Mangement.Resx;
using Library_Mangement.Services;
using Library_Mangement.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static Android.Provider.ContactsContract;

namespace Library_Mangement.ViewModels
{
    public class ProfileViewViewModel : ValidatableBase
    {
        #region Properties
        public string ProfilePath { get; set; }
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

        private ImageSource _defaultProfile = "user.png";
        public ImageSource DefaultProfile
        {
            get => _defaultProfile;
            set
            {
                _defaultProfile = value;
                OnPropertyChanged(nameof(DefaultProfile));
            }
        }
        #endregion

        #region Contructor
        public ProfileViewViewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand ChangeProfileCommand => new Command(() => ChangeProfileClicked());
        public ICommand UpdateUserCommand => new Command(async () => await UpdateUserClicked());
        #endregion

        #region Event Handlers
        private async void ChangeProfileClicked()
        {
            string filePath = await Common.ClickImageAndGetPath();
            if (!string.IsNullOrEmpty(filePath))
            {
                DefaultProfile = filePath;
                ProfilePath = filePath;
                App.CurrentLoggedInUser.ProfilePicPath = ProfilePath;
            }
            if (!string.IsNullOrEmpty(ProfilePath))
            {
                UserDialogs.Instance.ShowLoading();
                var imgUploadResp = await RestService.FileUpload(filePath, AppConfig.profile);
                UserDialogs.Instance.HideLoading();
                if (imgUploadResp != null)
                {
                    await App.Current.MainPage.DisplayAlert($"{imgUploadResp.StatusCode}", imgUploadResp.StatusDescription, AppResources.Ok);
                }
            }
        }

        private async Task UpdateUserClicked()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("");
                ValidateModel();
                var isFailedFieldAvailable = FieldItems.Any(x => x.IsFieldValidationFailed);
                if (!isFailedFieldAvailable)
                {
                    tblUser userModel = GetUpdatedUserData();
                    UserRegistrationPost uploadUser = GetUserRegistrationModel();
                    if (userModel != null)
                    {
                        UserRegistrationApiResp resp = await RestService.UpdateUser(uploadUser);
                        if (resp != null && resp.data != null)
                        {
                            await App.Database.User.InsertAsync(userModel);
                            App.CurrentLoggedInUser = userModel;
                            await App.Current.MainPage.DisplayAlert($"{resp.statusCode}", resp.message, AppResources.Ok);
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert($"Alert", "Please Fix Validation Errors !!!! Then Try Again", AppResources.Ok);
                    }
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
            }
            UserDialogs.Instance.HideLoading();
        }
        #endregion

        #region Public Methods
        public async Task LoadRegistrationForm(StackLayout profileStack)
        {
            UserDialogs.Instance.ShowLoading();
            if (FieldItems != null)
            {
                FieldItems.Clear();
            }
            FieldItems = new ObservableCollection<DynamicPropertyDataViewModel>();
            DefaultProfile = App.CurrentLoggedInUser.ProfilePicPath;
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
                    dynamicProperty.GroupName = fieldItems.GroupName;
                    dynamicProperty.KeyboardType = Common.GetKeyboardType(fieldItems.KeyboardType);
                    dynamicProperty.InputType = fieldItems.KeyboardType;
                    dynamicProperty.ControlType = fieldItems.ControlType;
                    dynamicProperty.PageName = fieldItems.PageName;
                    dynamicProperty.Validation = fieldItems.Validation;
                    dynamicProperty.ValidationMsg = fieldItems.ValidationMsg;
                    dynamicProperty.ListValues = fieldItems.ListValues;
                    dynamicProperty.FieldValue = GetValueForField(dynamicProperty.FieldId);
                    if (dynamicProperty.GroupName == "Password")
                    {
                        dynamicProperty.Required = false;
                    }
                    FieldItems.Add(dynamicProperty);
                }
                DynamicControlsView uiLoader = new DynamicControlsView();
                await uiLoader.LoadView(profileStack, FieldItems);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
            }
            UserDialogs.Instance.HideLoading();
        }

        #endregion

        #region Private Methods

        private string GetValueForField(int fieldId)
        {
            switch (fieldId)
            {
                case 101:
                    return App.CurrentLoggedInUser.FirstName;
                    break;

                case 102:
                    return App.CurrentLoggedInUser.LastName;
                    break;

                case 103:
                    return App.CurrentLoggedInUser.UserName;
                    break;

                case 104:
                    return App.CurrentLoggedInUser.RollNo.ToString();
                    break;

                case 105:
                    return App.CurrentLoggedInUser.Email;
                    break;

                case 106:
                    return App.CurrentLoggedInUser.Phone;
                    break;

                case 107:
                    return App.CurrentLoggedInUser.DOB.ToString();
                    break;

                case 108:
                    return App.CurrentLoggedInUser.Gender;
                    break;

                case 109:
                    return App.CurrentLoggedInUser.CollageName;
                    break;

                case 110:
                    return App.CurrentLoggedInUser.Education;
                    break;

                case 113:
                    return App.CurrentLoggedInUser.Catagories;
                    break;

                case 111:
                    return App.CurrentLoggedInUser.Password;
                    break;

                case 112:
                    return App.CurrentLoggedInUser.Password;
                    break;

                default:
                    return string.Empty;
                    break;
            }
        }
        private void ValidateModel()
        {
            foreach (var FieldItem in FieldItems)
            {
                if (FieldItem.Required)
                {
                    FieldItem.IsFieldValidationFailed = Common.ValidateInputField(FieldItem);
                }
                else
                {
                    FieldItem.IsFieldValidationFailed = false;
                }

            }

            var groupFields = FieldItems.Where(x => !string.IsNullOrEmpty(x.GroupName)).ToList();
            if (groupFields?.Count > 0)
            {
                bool isMatched = groupFields[0].FieldValue == groupFields[1].FieldValue ? false : true;
                if (isMatched)
                {
                    var result = FieldItems.FirstOrDefault(x => x.FieldId == groupFields[1].FieldId);
                    if (result != null)
                    {
                        result.IsFieldValidationFailed = true;
                        result.ValidationMsg = "Please Enter The Same Password";
                    }
                }
            }

        }

        private tblUser GetUpdatedUserData()
        {
            tblUser user = new tblUser();
            try
            {
                user.RollNo = Convert.ToInt32(FieldItems.FirstOrDefault(x => x.FieldId == 104).FieldValue);
                user.FirstName = FieldItems.FirstOrDefault(x => x.FieldId == 101).FieldValue;
                user.LastName = FieldItems.FirstOrDefault(x => x.FieldId == 102).FieldValue;
                user.UserName = FieldItems.FirstOrDefault(x => x.FieldId == 103).FieldValue;
                user.Email = FieldItems.FirstOrDefault(x => x.FieldId == 105).FieldValue;
                user.Phone = FieldItems.FirstOrDefault(x => x.FieldId == 106).FieldValue;
                user.CollageName = FieldItems.FirstOrDefault(x => x.FieldId == 109).FieldValue;
                user.Education = FieldItems.FirstOrDefault(x => x.FieldId == 110).FieldValue;
                user.Password = FieldItems.FirstOrDefault(x => x.FieldId == 111).FieldValue;
                user.ProfilePicPath = ProfilePath;
                user.Catagories = FieldItems.FirstOrDefault(x => x.FieldId == 113).FieldValue;
                user.Gender = FieldItems.FirstOrDefault(x => x.FieldId == 108).FieldValue;
                user.DOB = Convert.ToDateTime(FieldItems.FirstOrDefault(x => x.FieldId == 107).FieldValue);
            }
            catch (Exception ex)
            {

            }
            return user;
        }

        private UserRegistrationPost GetUserRegistrationModel()
        {
            var deviceInfo = Common.DeviceDetails();
            UserRegistrationPost userRegistration = new UserRegistrationPost();
            try
            {
                if (!string.IsNullOrEmpty(FieldItems.FirstOrDefault(x => x.FieldId == 111).FieldValue))
                {
                    userRegistration.Password = FieldItems.FirstOrDefault(x => x.FieldId == 111).FieldValue;
                }
                userRegistration.RollNo = Convert.ToInt32(FieldItems.FirstOrDefault(x => x.FieldId == 104).FieldValue);
                userRegistration.FirstName = FieldItems.FirstOrDefault(x => x.FieldId == 101).FieldValue;
                userRegistration.LastName = FieldItems.FirstOrDefault(x => x.FieldId == 102).FieldValue;
                userRegistration.UserName = FieldItems.FirstOrDefault(x => x.FieldId == 103).FieldValue;
                userRegistration.Email = FieldItems.FirstOrDefault(x => x.FieldId == 105).FieldValue;
                userRegistration.Password = FieldItems.FirstOrDefault(x => x.FieldId == 111).FieldValue;
                userRegistration.ProfileAvatar = "";
                userRegistration.DeviceId = deviceInfo.DeviceID;
                userRegistration.Phone = FieldItems.FirstOrDefault(x => x.FieldId == 106).FieldValue;
                userRegistration.CollageName = FieldItems.FirstOrDefault(x => x.FieldId == 109).FieldValue;
                userRegistration.CurrentEducation = FieldItems.FirstOrDefault(x => x.FieldId == 110).FieldValue;
                userRegistration.Catagories = string.IsNullOrEmpty(FieldItems.FirstOrDefault(x => x.FieldId == 113).FieldValue) ? "" : FieldItems.FirstOrDefault(x => x.FieldId == 113).FieldValue;
                userRegistration.Gender = FieldItems.FirstOrDefault(x => x.FieldId == 108).FieldValue;
                userRegistration.BirthDate = Convert.ToDateTime(FieldItems.FirstOrDefault(x => x.FieldId == 107).FieldValue);
            }
            catch (Exception ex)
            {

            }
            return userRegistration;
        }
        #endregion
    }
}
