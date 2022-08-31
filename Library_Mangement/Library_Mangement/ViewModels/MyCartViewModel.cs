using Acr.UserDialogs;
using Library_Mangement.Database.Models;
using Library_Mangement.Model.UIBinding;
using Library_Mangement.Validation;
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
    public class MyCartViewModel : ValidatableBase
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
        #endregion

        #region Constructor
        public MyCartViewModel()
        {

        }
        #endregion
        #region Commands
        public ICommand RemoveCommand => new Command(async(frame) => await RemoveClicked(frame as Frame));
        public ICommand BorrowCommand => new Command(async(frame) => await BorrowClicked(frame as Frame));
        #endregion

        #region Event Handlers
        private async Task RemoveClicked(Frame frame)
        {
            try
            {
                var bookDetails = frame.BindingContext as BookActionBinding;
                if (bookDetails != null)
                {
                    if (await App.Database.AddToCart.RemoveBookFromCart(App.CurrentLoggedInUser.UserID, bookDetails.BookActionId))
                    {
                        UserDialogs.Instance.Toast($"{bookDetails.BookTitle} is Removed From Cart");
                    }
                    else
                    {
                        UserDialogs.Instance.Toast($"Unable to remove {bookDetails.BookTitle}");
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
        private async Task BorrowClicked(Frame frame)
        {
            try
            {
                await RemoveClicked(frame);
                var bookDetails = frame.BindingContext as BookActionBinding;
                if (bookDetails != null)
                {
                    tblBorrowBook borrowBook = new tblBorrowBook()
                    {
                        BookISBN = bookDetails.BookISBN,
                        BorrowDate = DateTime.Now,
                        BorrowDays = 7,
                        BorrowID = bookDetails.BookActionId,
                        EndBorrowDate = DateTime.Now.AddDays(7),
                        RollNo = App.CurrentLoggedInUser.RollNo,
                        UserID = App.CurrentLoggedInUser.UserID,
                    };
                    await App.Current.MainPage.DisplayAlert("Alert", $"{bookDetails.BookTitle} is issued for you.", "OK");
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Public Methods
        public async Task LoadCartBooks()
        {
            try
            {
                if (MyBooks != null && MyBooks?.Count > 0)
                    return;
                MyBooks = new ObservableCollection<BookActionBinding>();
                var cartBooks = await App.Database.AddToCart.GetCartBooks(App.CurrentLoggedInUser.UserID, "cart");
                foreach (var cartItem in cartBooks)
                {
                    var bookData = await App.Database.Book.GetBooksByISBN(cartItem.BookISBN);
                    BookActionBinding bookAction = new BookActionBinding();
                    bookAction.BookAuther = bookData != null ? bookData.Authors : "Authors: NA";
                    bookAction.BookImage = bookData != null && File.Exists(bookData.PngFilePath) ? bookData.PngFilePath : "No_Thumb.png";
                    bookAction.BookTitle = bookData != null ? bookData.Title : "NA";
                    bookAction.BookPublishYear = bookData != null ? $"Publish: {bookData.PublishedDate}" : "Publish: NA";
                    bookAction.BookIssueDays = "Days: 7";
                    bookAction.BookActionId = !string.IsNullOrEmpty(cartItem.CartID) ? cartItem.CartID : Guid.NewGuid().ToString(); ;
                    MyBooks.Add(bookAction);
                }
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
