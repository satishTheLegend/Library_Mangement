using Acr.UserDialogs;
using Library_Mangement.Helper;
using Library_Mangement.Model.UIBinding;
using Library_Mangement.Services;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class MyBooksViewModel : ValidatableBase
    {
        #region Properties
        private ObservableCollection<BookActionBinding> _myBooks;
        public ObservableCollection<BookActionBinding> MyBooks
        {
            get => _myBooks;
            set
            {
                _myBooks = value;
                OnPropertyChanged(nameof(MyBooks));
            }
        }
        private bool _loaderVisible = false;
        public bool LoaderVisible
        {
            get => _loaderVisible;
            set
            {
                _loaderVisible = value;
                FlexViewIsVisible = !value;
                OnPropertyChanged(nameof(LoaderVisible));
            }
        }

        private double? _loaderPercent;
        public double? LoaderPercent
        {
            get => _loaderPercent;
            set
            {
                _loaderPercent = value;
                OnPropertyChanged(nameof(_loaderPercent));
            }
        }

        private bool _flexViewIsVisible = true;
        public bool FlexViewIsVisible
        {
            get => _flexViewIsVisible;
            set
            {
                _flexViewIsVisible = value;
                OnPropertyChanged(nameof(FlexViewIsVisible));
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
        public MyBooksViewModel()
        {
            LoaderVisible = false;
            RestService.ProgressEvent += RestServiceConnection_ProgressEvent;
        }
        #endregion

        #region Commands
        public ICommand ReturnBookCommand => new Command(async (frame) => await ReturnBookClickedAsync(frame as Frame));
        public ICommand ViewBookCommand => new Command(async (frame) => await ViewBookClicked(frame as Frame));
        #endregion

        #region Event Handlers
        private async Task ReturnBookClickedAsync(Frame frame)
        {
            try
            {
                var bookData = frame.BindingContext as BookActionBinding;
                if (bookData != null && !string.IsNullOrEmpty(bookData.BookISBN))
                {
                    if (await App.Database.BorrowBook.UpdateBookStatus(App.CurrentLoggedInUser.UserID, bookData.BookActionId))
                    {
                        await App.Current.MainPage.DisplayAlert("Attention", $"{bookData.BookTitle} is Returned", "OK");
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Attention", $"Unable to return book {bookData.BookTitle}", "OK");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task ViewBookClicked(Frame frame)
        {
            LoaderVisible = true;
            try
            {
                await LoaderMessage($"Initalizing your book !!!", 200);
                var bookDetails = frame.BindingContext as BookActionBinding;
                if (bookDetails != null)
                {
                    var bookData = await App.Database.Book.GetBooksByISBN(bookDetails.BookISBN);
                    if (bookData != null && !string.IsNullOrEmpty(bookData.PdfFilePath))
                    {
                        await LoaderMessage($"Opening Book, Please Wait", 200);
                        await App.Current.MainPage.Navigation.PushAsync(new BookPDFView(bookData.PdfFilePath));
                    }
                    else
                    {
                        await LoaderMessage($"Downloading Your Book !!!", 200);
                        string bookpdfDirectoryPath = Common.GetBasePath(AppConfig.DirName_Books_PDFFiles);
                        if (await RestService.DownloadUrlFiles(bookData.PdfLink, bookpdfDirectoryPath, $"{bookData.PdfName}.pdf", true))
                        {
                            bookData.PdfFilePath = Path.Combine(bookpdfDirectoryPath, $"{bookData.PdfName}.pdf");
                            await App.Database.Book.InsertAsync(bookData);
                            await App.Current.MainPage.Navigation.PushAsync(new BookPDFView(bookData.PdfFilePath));
                        }
                        else
                        {
                            UserDialogs.Instance.Toast($"{bookData.Title} unable to download !!! Please try again");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LoaderVisible = false;
            }
            LoaderVisible = false;
        }
        private void RestServiceConnection_ProgressEvent(double? value)
        {
            LoaderPercent = value;
        }
        #endregion

        #region Public Methods
        public async Task LoadMyBooks()
        {
            LoaderVisible = true;
            try
            {
                if (MyBooks != null && MyBooks?.Count > 0)
                {
                    LoaderVisible = false;
                    return;
                }

                MyBooks = new ObservableCollection<BookActionBinding>();
                var myBooks = await App.Database.BorrowBook.GetMyBooks(App.CurrentLoggedInUser.UserID);
                foreach (var myBookItem in myBooks)
                {
                    var bookData = await App.Database.Book.GetBooksByISBN(myBookItem.BookISBN);
                    BookActionBinding bookAction = new BookActionBinding();
                    bookAction.BookAuther = bookData != null ? bookData.Authors : "Auther: NA";
                    bookAction.BookImage = bookData != null && File.Exists(bookData.PngFilePath) ? bookData.PngFilePath : "No_Thumb.png";
                    bookAction.BookIssueDate = bookData != null ? $"Issue Date: {bookData.PublishedDate}" : "Issue Date: NA";
                    bookAction.BookReturnDate = bookData != null ? $"Return Date: {bookData.PublishedDate}" : "Return Date: NA";
                    bookAction.BookISBN = bookData != null ? bookData.ISBN : string.Empty;
                    bookAction.BookActionId = !string.IsNullOrEmpty(myBookItem.BorrowID) ? myBookItem.BorrowID : Guid.NewGuid().ToString();
                    MyBooks.Add(bookAction);
                }
            }
            catch (Exception ex)
            {
                LoaderVisible = false;
            }
            LoaderVisible = false;
        }
        #endregion

        #region Private Methods
        private async Task LoaderMessage(string loaderText, int timeDeley, bool isStopLoader = false)
        {
            LoaderText = $"{loaderText}";
            if (timeDeley > 0)
            {
                await Task.Delay(timeDeley);
            }
        }

        #endregion
    }
}
