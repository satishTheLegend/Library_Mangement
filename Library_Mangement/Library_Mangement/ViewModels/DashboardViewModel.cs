using Acr.UserDialogs;
using Library_Mangement.Controls;
using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model;
using Library_Mangement.Model.ApiResponse;
using Library_Mangement.Model.ApiResponse.PostModels;
using Library_Mangement.Model.UIBinding;
using Library_Mangement.Resx;
using Library_Mangement.Services;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class DashboardViewModel : ValidatableBase, INotifyPropertyChanged
    {
        #region Properties
        List<tblBook> bookList = null;
        public List<Menu> MyMenu { get; set; }
        public string ProfileImg { get; set; } = string.Empty;

        public DashboardBinding dashboard { get; set; }
        public ProfileBinding profile { get; set; }

        private ObservableCollection<tblBook> _books;
        public ObservableCollection<tblBook> Books
        {
            get => _books;
            set
            {
                _books = value;
                //LoaderVisible = true;
                //try
                //{
                //    if (Books?.Count > 0)
                //    {
                //        LottieAnimationName = "Downloading_Files.json";
                //        LoaderVisible = false;
                //    }
                //    else
                //    {
                //        LottieAnimationName = "Data_NotFound.json";
                //        LoaderText = "OOPS !!!! We didnt found your book, Sorry !";
                //    }
                //}
                //catch (Exception ex)
                //{

                //}
                OnPropertyChanged(nameof(Books));
            }
        }

        private bool _loaderVisible = false;
        public bool LoaderVisible
        {
            get => _loaderVisible;
            set
            {
                _loaderVisible = value;
                HideCards = !value;
                OnPropertyChanged(nameof(LoaderVisible));
            }
        }

        private bool _hideCards = false;
        public bool HideCards
        {
            get => _hideCards;
            set
            {
                _hideCards = value;
                OnPropertyChanged(nameof(HideCards));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }
        private string _lottieAnimationName = "Downloading_Files.json";
        public string LottieAnimationName
        {
            get => _lottieAnimationName;
            set
            {
                _lottieAnimationName = value;
                OnPropertyChanged(nameof(LottieAnimationName));
            }
        }

        public void ShowForm()
        {
            UserDialogs.Instance.ShowLoading();
            dashboard.DashboardVisible = false;
            profile.ProfileVisible = true;
            UserDialogs.Instance.HideLoading();
        }

        private string _loaderText;
        public string LoaderText
        {
            get => _loaderText;
            set
            {
                _loaderText = value;
                OnPropertyChanged(nameof(LoaderText));
            }
        }
        #endregion

        #region Constructor
        public DashboardViewModel()
        {
            MyMenu = GetMenus();
            LoadUserData();
        }

        private void LoadUserData()
        {
            dashboard = new DashboardBinding();
            profile = new ProfileBinding();
            dashboard.UserName = $"{App.CurrentLoggedInUser.FirstName} {App.CurrentLoggedInUser.LastName}";
            if (!string.IsNullOrEmpty(App.CurrentLoggedInUser.ProfilePicPath) && File.Exists(App.CurrentLoggedInUser.ProfilePicPath))
            {
                dashboard.ProfileImage = App.CurrentLoggedInUser.ProfilePicPath;
            }
        }
        #endregion

        #region Commands
        public ICommand OpenBookCommand => new Command(frame => OpenBookClicked(frame as Frame));
        public ICommand SearchCommand => new Command(() => SearchClicked());
        public ICommand ChangeProfileCommand => new Command(() => ChangeProfileClicked());


        public ICommand UpdateUserCommand => new Command(async() => await UpdateUserClicked());
        #endregion

        #region Event Handlers
        private async void OpenBookClicked(Frame frame)
        {
            var bookData = frame.BindingContext as BooksPropertyModel;
            if (bookData != null)
            {
                LoaderVisible = true;
                await LoaderMessage($"Opening {bookData.Title}", 1500);
                tblBook book = await App.Database.Book.GetBookByISBNId(bookData.ISBN);
                await App.Current.MainPage.Navigation.PushAsync(new BookView(book));
                LoaderVisible = false;
            }
        }
        private async Task UpdateUserClicked()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("");
                ValidateModel();
                var isFailedFieldAvailable = profile.FieldItems.Any(x => x.IsFieldValidationFailed);
                if (!isFailedFieldAvailable)
                {
                    tblUser userModel = GetUpdatedUserData();
                    UserRegistrationPost uploadUser = GetUserRegistrationModel();
                    if (userModel != null)
                    {
                        UserRegistrationApiResp resp = await RestService.UpdateUser(uploadUser);
                        if (resp != null && resp.data != null)
                        {
                            if (!string.IsNullOrEmpty(App.CurrentLoggedInUser.ProfilePicPath) && File.Exists(App.CurrentLoggedInUser.ProfilePicPath))
                            {
                                dashboard.ProfileImage = App.CurrentLoggedInUser.ProfilePicPath;
                            }
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
            dashboard.DashboardVisible = false;
            profile.ProfileVisible = true;
            if(profile.FieldItems != null)
            {
                profile.FieldItems.Clear();
            }
            if(dashboard.ProfileImage != null)
            {
                profile.DefaultProfile = dashboard.ProfileImage;
            }
            profile.FieldItems = new ObservableCollection<DynamicPropertyDataViewModel>();
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
                    if(dynamicProperty.GroupName == "Password")
                    {
                        dynamicProperty.Required = false;
                    }
                    profile.FieldItems.Add(dynamicProperty);
                }
                DynamicControlsView uiLoader = new DynamicControlsView();
                await uiLoader.LoadView(profileStack, profile.FieldItems);
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();
            }
            UserDialogs.Instance.HideLoading();
        }

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

        public async Task LoadBooksInfo_Updated()
        {
            try
            {
                LoaderVisible = true;
                HideCards = true;
                await LoaderMessage($"Getting Books From Database", 1300);

                List<tblBook> allBooks = await App.Database.Book.GetDataAsync();
                if (allBooks?.Count > 0)
                {
                    await LoaderMessage($"Fetched {allBooks.Count} Books From Database", 1300);
                    await LoaderMessage($"Arrenging Books Please Wait ....", 1300);
                    //await GetBookList(allBooks);
                    Books = new ObservableCollection<tblBook>(allBooks);
                }
                LoaderVisible = false;
                HideCards = false;
            }
            catch (Exception ex)
            {

            }
        }

        private async Task GetBookList(List<tblBook> allBooks)
        {
            //try
            //{
            //    int i = 0;
            //    foreach (var bookItem in allBooks)
            //    {
            //        i++;
            //        BooksPropertyModel book = new BooksPropertyModel()
            //        {
            //            Title = bookItem.Title,
            //            ISBN = bookItem.ISBN,
            //            PageCount = bookItem.PageCount,
            //            Auther = bookItem.Authors,
            //            Catagory = bookItem.Categories,
            //            PublishYear = bookItem.PublishedDate,
            //        };
            //        Books.Add(book);
            //        await LoaderMessage($"Adding Books To View Completed {i} out of {allBooks.Count} ....", 1300);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    // Book_ImageSource = File.Exists(bookItem.PngFilePath) ? bookItem.PngFilePath : "PlaceHolder.png",
            //}
        }

        //public async Task LoadBooksInfo()
        //{
        //    try
        //    {
        //        LoaderVisible = true;
        //        await LoaderMessage($"Getting Books From Database", 1300);

        //        if (allBooks?.Count > 0)
        //        {
        //            await LoaderMessage($"Fetched {allBooks.Count} Books From Database", 1300);
        //            await LoaderMessage($"Arrenging Books Please Wait ....", 1300);
        //            bookList = await LoadBooksFromTable(allBooks);
        //            if (bookList?.Count > 0)
        //            {

        //            }
        //                //Books = new ObservableCollection<BooksPropertyModel>(bookList);
        //            //await LoadFinalTextOfLoader();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        LoaderVisible = false;
        //    }
        //    LoaderVisible = false;
        //}
        public void SearchClicked()
        {
            try
            {
                if (!string.IsNullOrEmpty(SearchText))
                {
                    var bookItems = bookList.Where(x => (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(SearchText)) || (!string.IsNullOrEmpty(x.ISBN) && x.ISBN.Contains(SearchText))).ToList();
                    LoaderVisible = true;
                    LottieAnimationName = "Data_NotFound.json";
                    LoaderText = "Please Wait";
                    Books.Clear();
                    LoaderVisible = false;
                    if (bookItems?.Count > 0)
                    {
                        //Books = new ObservableCollection<BooksPropertyModel>(bookItems);
                    }
                    else
                    {
                        LottieAnimationName = "Data_NotFound.json";
                        LoaderVisible = true;
                        LoaderText = "OOPS !!!! We didnt found your book, Sorry !";
                    }
                }
                else
                {
                    LoaderVisible = true;
                    LoaderText = "OOPS !!!! We didnt found your book, Sorry !";
                    Books.Clear();
                    //Books = new ObservableCollection<BooksPropertyModel>(bookList);
                    LoaderVisible = false;
                    LottieAnimationName = "Downloading_Files.json";
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Private Methods
        private tblUser GetUpdatedUserData()
        {
            tblUser user = new tblUser();
            try
            {
                user.RollNo = Convert.ToInt32(profile.FieldItems.FirstOrDefault(x => x.FieldId == 104).FieldValue);
                user.FirstName = profile.FieldItems.FirstOrDefault(x => x.FieldId == 101).FieldValue;
                user.LastName = profile.FieldItems.FirstOrDefault(x => x.FieldId == 102).FieldValue;
                user.UserName = profile.FieldItems.FirstOrDefault(x => x.FieldId == 103).FieldValue;
                user.Email = profile.FieldItems.FirstOrDefault(x => x.FieldId == 105).FieldValue;
                user.Phone = profile.FieldItems.FirstOrDefault(x => x.FieldId == 106).FieldValue;
                user.CollageName = profile.FieldItems.FirstOrDefault(x => x.FieldId == 109).FieldValue;
                user.Education = profile.FieldItems.FirstOrDefault(x => x.FieldId == 110).FieldValue;
                user.Password = profile.FieldItems.FirstOrDefault(x => x.FieldId == 111).FieldValue;
                user.ProfilePicPath = ProfileImg;
                user.Catagories = profile.FieldItems.FirstOrDefault(x => x.FieldId == 113).FieldValue;
                user.Gender = profile.FieldItems.FirstOrDefault(x => x.FieldId == 108).FieldValue;
                user.DOB = Convert.ToDateTime(profile.FieldItems.FirstOrDefault(x => x.FieldId == 107).FieldValue);
            }
            catch (Exception ex)
            {

            }
            return user;
        }

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
        private UserRegistrationPost GetUserRegistrationModel()
        {
            var deviceInfo = Common.DeviceDetails();
            UserRegistrationPost userRegistration = new UserRegistrationPost();
            try
            {
                if(!string.IsNullOrEmpty(profile.FieldItems.FirstOrDefault(x => x.FieldId == 111).FieldValue))
                {
                    userRegistration.Password = profile.FieldItems.FirstOrDefault(x => x.FieldId == 111).FieldValue;
                }
                userRegistration.RollNo = Convert.ToInt32(profile.FieldItems.FirstOrDefault(x => x.FieldId == 104).FieldValue);
                userRegistration.FirstName = profile.FieldItems.FirstOrDefault(x => x.FieldId == 101).FieldValue;
                userRegistration.LastName = profile.FieldItems.FirstOrDefault(x => x.FieldId == 102).FieldValue;
                userRegistration.UserName = profile.FieldItems.FirstOrDefault(x => x.FieldId == 103).FieldValue;
                userRegistration.Email = profile.FieldItems.FirstOrDefault(x => x.FieldId == 105).FieldValue;
                userRegistration.Password = profile.FieldItems.FirstOrDefault(x => x.FieldId == 111).FieldValue;
                userRegistration.ProfileAvatar = "";
                userRegistration.DeviceId = deviceInfo.DeviceID;
                userRegistration.Phone = profile.FieldItems.FirstOrDefault(x => x.FieldId == 106).FieldValue;
                userRegistration.CollageName = profile.FieldItems.FirstOrDefault(x => x.FieldId == 109).FieldValue;
                userRegistration.CurrentEducation = profile.FieldItems.FirstOrDefault(x => x.FieldId == 110).FieldValue;
                userRegistration.Catagories = string.IsNullOrEmpty(profile.FieldItems.FirstOrDefault(x => x.FieldId == 113).FieldValue) ? "" : profile.FieldItems.FirstOrDefault(x => x.FieldId == 113).FieldValue;
                userRegistration.Gender = profile.FieldItems.FirstOrDefault(x => x.FieldId == 108).FieldValue;
                userRegistration.BirthDate = Convert.ToDateTime(profile.FieldItems.FirstOrDefault(x => x.FieldId == 107).FieldValue);
            }
            catch (Exception ex)
            {

            }
            return userRegistration;
        }
        private async void ChangeProfileClicked()
        {
            string filePath = await Common.ClickImageAndGetPath();
            if(!string.IsNullOrEmpty(filePath))
            {
                profile.DefaultProfile = filePath;
                profile.DefaultProfile = filePath;
                ProfileImg = filePath;
            }
            if(!string.IsNullOrEmpty(ProfileImg))
            {
                UserDialogs.Instance.ShowLoading();
                var imgUploadResp = await RestService.FileUpload(ProfileImg, AppConfig.profile);
                UserDialogs.Instance.HideLoading();
                if (imgUploadResp != null)
                {
                    await App.Current.MainPage.DisplayAlert($"{imgUploadResp.StatusCode}", imgUploadResp.StatusDescription, AppResources.Ok);
                }
            }
        }

        private async Task<List<BooksPropertyModel>> LoadBooksFromTable(List<tblBook> allBooks)
        {
            List<BooksPropertyModel> books = new List<BooksPropertyModel>();
            try
            {
                int bookCount = 0;
                if (Books != null) Books.Clear();

                foreach (var bookItem in allBooks)
                {
                    if (!File.Exists(bookItem.PngFilePath))
                    {
                        bool isSaved = await RestService.DownloadFileFromURIAndSaveIt(bookItem.PngFilePath, bookItem.PngFilePath);
                    }
                    BooksPropertyModel book = new BooksPropertyModel()
                    {
                        ISBN = bookItem.ISBN,
                        Book_ImageSource = bookItem.IsCoverAvailable ? bookItem.PngFilePath : "PlaceHolder.png",
                        IsCoverAvailable = bookItem.IsCoverAvailable,
                        PageCount = bookItem.PageCount,
                        Title = bookItem.Title,
                    };
                    if (bookCount == allBooks.Count - 1)
                    {
                        await LoaderMessage($"Loading...", 0);
                    }
                    else
                    {
                        await LoaderMessage($"Added Books To View {bookCount} out of {allBooks.Count}", 300);
                    }
                    bookCount++;
                    books.Add(book);
                }
            }
            catch (Exception ex)
            {

            }
            return books;
        }
        private async Task LoadFinalTextOfLoader()
        {
            try
            {
                string finishingText = "Please Wait Finishing Up";
                for (int i = 0; i < 12; i++)
                {
                    switch (i)
                    {
                        case 8:
                            finishingText = "Finializing";
                            break;

                        case 10:
                            finishingText = "Process Completed";
                            break;
                    }

                    for (int j = 1; j <= 4; j++)
                    {
                        string progress = string.Concat(Enumerable.Repeat(".", j));
                        await LoaderMessage($"{finishingText}{progress}", 200);
                    }
                }
                await LoaderMessage($"", 400, true);
            }
            catch (Exception ex)
            {

            }

        }
        private async Task LoaderMessage(string loaderText, int timeDeley, bool isStopLoader = false)
        {
            LoaderText = $"{loaderText}";
            if (timeDeley > 0 && AppConfig.isAwaitTimeNeeds)
            {
                await Task.Delay(timeDeley);
            }
        }
        #region Private Methods
        private void ValidateModel()
        {
            foreach (var FieldItem in profile.FieldItems)
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

            var groupFields = profile.FieldItems.Where(x => !string.IsNullOrEmpty(x.GroupName)).ToList();
            if (groupFields?.Count > 0)
            {
                bool isMatched = groupFields[0].FieldValue == groupFields[1].FieldValue ? false : true;
                if (isMatched)
                {
                    var result = profile.FieldItems.FirstOrDefault(x => x.FieldId == groupFields[1].FieldId);
                    if (result != null)
                    {
                        result.IsFieldValidationFailed = true;
                        result.ValidationMsg = "Please Enter The Same Password";
                    }
                }
            }

        }
        #endregion
        #endregion

    }
    public class Menu
    {
        public string Name { get; set; }
        public string Icon { get; set; }
    }

}
