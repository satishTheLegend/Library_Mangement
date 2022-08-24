using Acr.UserDialogs;
using Library_Mangement.Database.Models;
using Library_Mangement.Model;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using Library_Mangement.Views.Cards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class BookViewModel : ValidatableBase
    {
        #region Properties
        private string _loaderText = string.Empty;
        public string LoaderText
        {
            get => _loaderText;
            set
            {
                _loaderText = value;
                OnPropertyChanged(nameof(LoaderText));
            }
        }

        //private ObservableCollection<BooksGroup> _books;
        //public ObservableCollection<BooksGroup> Books
        //{
        //    get => _books;
        //    set
        //    {
        //        _books = value;
        //        OnPropertyChanged(nameof(_books));
        //    }
        //}
        private ObservableCollection<BooksPropertyModel> _books;
        public ObservableCollection<BooksPropertyModel> Books
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged(nameof(_books));
            }
        }

        private bool _isbooksVisible = false;
        public bool IsbooksVisible
        {
            get => _isbooksVisible;
            set
            {
                _isbooksVisible = value;
                OnPropertyChanged(nameof(_isbooksVisible));
            }
        }
        #endregion

        #region Constructor
        public BookViewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand OpenBookCommand => new Command(frame => OpenBookClicked(frame as Frame));


        #endregion

        #region Event Handlers
        private void OpenBookClicked(Frame frame)
        {

        }
        #endregion

        #region Public Methods
        public async Task LoadBooks()
        {
            UserDialogs.Instance.ShowLoading(LoaderText);
            await LoadBooksInfo_Updated();
            UserDialogs.Instance.HideLoading();

        }
        #endregion

        #region Private Methods
        public async Task LoadBooksInfo_Updated()
        {
            try
            {
                //LoaderVisible = true;
                //HideCards = true;
                await LoaderMessage($"Getting Books From Database", 1300);

                List<tblBook> allBooks = await App.Database.Book.GetDataAsync();
                if (allBooks?.Count > 0)
                {
                    await LoaderMessage($"Fetched {allBooks.Count} Books From Database", 1300);
                    await LoaderMessage($"Arrenging Books Please Wait ....", 1300);
                    var booksModelList = await GetBookList(allBooks);

                    //var catagorygroup = booksModelList.GroupBy(x => x.Catagory).ToList();
                    //List<BooksGroup> booksList = new List<BooksGroup>();
                    //foreach (var catagoryItem in catagorygroup)
                    //{
                    //    ObservableCollection<BooksPropertyModel> bookCatagory = new ObservableCollection<BooksPropertyModel>(catagoryItem);
                    //    booksList.Add(new BooksGroup(catagoryItem.Key.ToString(), bookCatagory));
                    //}
                    //Books = new ObservableCollection<BooksGroup>(booksList);
                    //await GetBookList(allBooks);
                    //Books = new ObservableCollection<tblBook>(allBooks);
                }
                //LoaderVisible = false;
                //HideCards = false;
            }
            catch (Exception ex)
            {

            }
        }

        private async Task LoaderMessage(string loaderText, int timeDeley, bool isStopLoader = false)
        {
            LoaderText = $"{loaderText}";

            //if (timeDeley > 0 && AppConfig.isAwaitTimeNeeds)
            //{
            //    await Task.Delay(timeDeley);
            //}
        }

        private async Task<List<BooksPropertyModel>> GetBookList(List<tblBook> allBooks)
        {
            List<BooksPropertyModel> booksList = new List<BooksPropertyModel>();
            try
            {
                Books = new ObservableCollection<BooksPropertyModel>();
                int i = 0;
                foreach (var bookItem in allBooks)
                {
                    string[] allFiels = Directory.GetFiles(Path.GetDirectoryName(bookItem.PngFilePath));
                    bool isFileExist = File.Exists(bookItem.PngFilePath);
                    i++;
                    BooksPropertyModel book = new BooksPropertyModel();
                    book.Title = bookItem.Title;
                    book.ISBN = bookItem.ISBN;
                    book.PageCount = bookItem.PageCount;
                    book.Auther = bookItem.Authors;
                    book.Book_ImageSource = File.Exists(bookItem.PngFilePath) ? bookItem.PngFilePath : "PlaceHolder.png";
                    book.Catagory = bookItem.Categories;
                    book.PublishYear = bookItem.PublishedDate;
                    book.IsCoverAvailable = bookItem.IsCoverAvailable;
                    Books.Add(book);
                    booksList.Add(book);
                    //await LoaderMessage($"Adding Books To View Completed {i} out of {allBooks.Count} ....", 1300);
                }
            }
            catch (Exception ex)
            {
                // Book_ImageSource = File.Exists(bookItem.PngFilePath) ? bookItem.PngFilePath : "PlaceHolder.png",
            }
            return booksList;
        }
        #endregion
    }
}
