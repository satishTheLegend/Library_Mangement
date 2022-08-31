using Acr.UserDialogs;
using Library_Mangement.Database.Models;
using Library_Mangement.Validation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class OpenBookViewModel : ValidatableBase
    {
        #region Properties
        private tblBook bookDetails;
        private ImageSource _bookImage = "No_Thumb.png";
        public ImageSource BookImage
        {
            get => _bookImage;
            set
            {
                _bookImage = value;
                OnPropertyChanged(nameof(BookImage));
            }
        }

        private string _bookCat;
        public string BookCat
        {
            get => _bookCat;
            set
            {
                _bookCat = value;
                OnPropertyChanged(nameof(BookCat));
            }
        }

        private string _bookAuther;
        public string BookAuther
        {
            get => _bookAuther;
            set
            {
                _bookAuther = value;
                OnPropertyChanged(nameof(BookAuther));
            }
        }

        private string _bookName;
        public string BookName
        {
            get => _bookName;
            set
            {
                _bookName = value;
                OnPropertyChanged(nameof(BookName));
            }
        }

        private string _bookPages;
        public string BookPages
        {
            get => _bookPages;
            set
            {
                _bookPages = value;
                OnPropertyChanged(nameof(BookPages));
            }
        }

        private string _bookLang;
        public string BookLang
        {
            get => _bookLang;
            set
            {
                _bookLang = value;
                OnPropertyChanged(nameof(BookLang));
            }
        }

        private string _bookDesc;
        public string BookDesc
        {
            get => _bookDesc;
            set
            {
                _bookDesc = value;
                OnPropertyChanged(nameof(BookDesc));
            }
        }

        private string _bookPublish;
        public string BookPublish
        {
            get => _bookPublish;
            set
            {
                _bookPublish = value;
                OnPropertyChanged(nameof(BookPublish));
            }
        }
        #endregion

        #region Constructor
        public OpenBookViewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand AddCartCommand => new Command(() => AddCartClicked());
        public ICommand BorrowCommand => new Command(() => BorrowClicked());
        public ICommand CloseCommand => new Command(() => CloseClicked());
        public ICommand WishListCommand => new Command(() => WishListClicked());
        #endregion

        #region Event Handlers
        private async Task AddCartClicked()
        {
            if (bookDetails != null)
            {
                tblAddToCart cartBook = new tblAddToCart()
                {
                    BookISBN = bookDetails.ISBN,
                    AddedtoCartDate = DateTime.Now,
                    CartID = Guid.NewGuid().ToString(),
                    Type = "cart",
                    RollNo = App.CurrentLoggedInUser.RollNo,
                    UserID = App.CurrentLoggedInUser.UserID,
                };
                await App.Database.AddToCart.InsertAsync(cartBook);
                await App.Current.MainPage.DisplayAlert("Alert", $"You have succesfully added {BookName} in your cart.", "OK");
            }
        }
        private async Task BorrowClicked()
        {
            if(bookDetails != null)
            {
                tblBorrowBook borrowBook = new tblBorrowBook()
                {
                    BookISBN = bookDetails.ISBN,
                    BorrowDate = DateTime.Now,
                    BorrowDays = 7,
                    BorrowID = Guid.NewGuid().ToString(),
                    EndBorrowDate = DateTime.Now.AddDays(7),
                    RollNo = App.CurrentLoggedInUser.RollNo,
                    UserID = App.CurrentLoggedInUser.UserID,
                };
                await App.Database.BorrowBook.InsertAsync(borrowBook);
                await App.Current.MainPage.DisplayAlert("Alert", $"{BookName} is issued for you.", "OK");
            }
        }
        private async void CloseClicked()
        {
            await App.Current.MainPage.Navigation.PopAsync();
        }

        private async Task WishListClicked()
        {
            tblAddToCart cartBook = new tblAddToCart()
            {
                BookISBN = bookDetails.ISBN,
                AddedtoCartDate = DateTime.Now,
                CartID = Guid.NewGuid().ToString(),
                Type = "wishlist",
                RollNo = App.CurrentLoggedInUser.RollNo,
                UserID = App.CurrentLoggedInUser.UserID,
            };
            await App.Database.AddToCart.InsertAsync(cartBook);
            await App.Current.MainPage.DisplayAlert("Alert", $"You have succesfully added {BookName} in your wishlist.", "OK");
        }

        #endregion

        #region Public Methods
        public async Task LoadBookDataAsync(tblBook BookData)
        {
            try
            {
                UserDialogs.Instance.ShowLoading();
                if (BookData != null)
                {
                    bookDetails = BookData;
                    BookImage = File.Exists(BookData.PngFilePath) ? BookData.PngFilePath : "No_Thumb.png";
                    BookCat = !string.IsNullOrEmpty(BookData.Categories) ? BookData.Categories : "NA";
                    BookAuther = !string.IsNullOrEmpty(BookData.Authors) ? BookData.Authors : "NA";
                    BookPublish = !string.IsNullOrEmpty(BookData.PublishedDate) ? BookData.PublishedDate : "NA";
                    BookName = !string.IsNullOrEmpty(BookData.Title) ? BookData.Title : "NA";
                    BookPages = BookData.PageCount > 0 ? BookData.PageCount.ToString() : "NA";
                    BookLang = "English";
                    BookDesc = "The book is an account of a 14-year period in Papillon's life (October 26, 1931 to October 18, 1945), beginning when he was wrongly convicted of murder in France and sentenced to a life of hard labor at the Bagne de Cayenne, the penal colony of Cayenne in French Guiana known as Devil's Island. The book is an account of a 14-year period in Papillon's life (October 26, 1931 to October 18, 1945), beginning when he was wrongly convicted of murder in France and sentenced to a life of hard labor at the Bagne de Cayenne, the penal colony of Cayenne in French Guiana known as Devil's Island.";

                }
                UserDialogs.Instance.HideLoading();
                await Task.FromResult(Task.CompletedTask);
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Private Methods

        #endregion
    }
}



//#region Properties

//#endregion

//#region Constructor

//#endregion

//#region Commands

//#endregion

//#region Event Handlers

//#endregion

//#region Public Methods

//#endregion

//#region Private Methods

//#endregion