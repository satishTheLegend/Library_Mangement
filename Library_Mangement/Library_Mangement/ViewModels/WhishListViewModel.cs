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
    public class WhishListViewModel : ValidatableBase
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
        public WhishListViewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand AddCartCommand => new Command(async (frame) => await AddCartClicked(frame as Frame));
        #endregion

        #region Event Handlers
        private async Task AddCartClicked(Frame frame)
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
                if (bookDetails != null)
                {
                    tblAddToCart cartBook = new tblAddToCart()
                    {
                        BookISBN = bookDetails.BookISBN,
                        AddedtoCartDate = DateTime.Now,
                        CartID = bookDetails.BookActionId,
                        Type = "cart",
                        RollNo = App.CurrentLoggedInUser.RollNo,
                        UserID = App.CurrentLoggedInUser.UserID,
                    };
                    await App.Database.AddToCart.InsertAsync(cartBook);
                    await App.Current.MainPage.DisplayAlert("Alert", $"You have succesfully added {bookDetails.BookTitle} in your cart.", "OK");
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Public Methods
        public async Task LoadWLBooks()
        {
            try
            {
                if (MyBooks != null && MyBooks?.Count > 0)
                    return;
                MyBooks = new ObservableCollection<BookActionBinding>();
                var wishListBooks = await App.Database.AddToCart.GetCartBooks(App.CurrentLoggedInUser.UserID, "wishlist");
                foreach (var wishBookItem in wishListBooks)
                {
                    var bookData = await App.Database.Book.GetBooksByISBN(wishBookItem.BookISBN);
                    BookActionBinding bookAction = new BookActionBinding();
                    bookAction.BookAuther = bookData != null ? bookData.Authors : "Auther: NA";
                    bookAction.BookImage = bookData != null && File.Exists(bookData.PngFilePath) ? bookData.PngFilePath : "No_Thumb.png";
                    bookAction.BookTitle = bookData != null ? bookData.Title : "NA";
                    bookAction.BookPublishYear = bookData != null ? $"Publish: {bookData.PublishedDate}" : "Publish: NA";
                    bookAction.BookCategory = bookData != null ? $"Category: {bookData.Categories}" : "Category: NA";
                    bookAction.BookISBN = bookData != null ? bookData.ISBN : string.Empty;
                    bookAction.BookActionId = !string.IsNullOrEmpty(wishBookItem.CartID) ? wishBookItem.CartID : Guid.NewGuid().ToString();
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
