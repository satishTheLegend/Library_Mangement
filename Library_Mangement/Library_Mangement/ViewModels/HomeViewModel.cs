using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model;
using Library_Mangement.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class HomeViewModel : ValidatableBase
    {
        #region Properties
        private ObservableCollection<BooksPropertyModel> _books;
        public ObservableCollection<BooksPropertyModel> Books
        {
            get => _books;
            set
            {
                _books = value;
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
                OnPropertyChanged(nameof(LoaderVisible));
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
        public HomeViewModel()
        {

        }

        #endregion

        #region Commands
        public ICommand OpenBookCommand => new Command(() => OpenBookClicked());

        #endregion

        #region Event Handlers
        private void OpenBookClicked()
        {
            //
        }
        #endregion

        #region Public Methods
        public async Task LoadBooksInfo()
        {
            LoaderVisible = true;
            await LoaderMessage($"Getting Books From Database", 1300);
            var allBooks = await App.Database.Book.GetDataAsync();
            if (allBooks?.Count > 0)
            {
                await LoaderMessage($"Fetched {allBooks.Count} Books From Database", 1300);
                await LoaderMessage($"Arrenging Books Please Wait ....", 1300);
                List<BooksPropertyModel> bookList = await LoadBooksFromTable(allBooks);
                if (bookList?.Count > 0)
                    Books = new ObservableCollection<BooksPropertyModel>(bookList);
            }
        }

        #endregion

        #region Private Methods
        private async Task<List<BooksPropertyModel>> LoadBooksFromTable(List<tblBook> allBooks)
        {
            List<BooksPropertyModel> books = new List<BooksPropertyModel>();
            try
            {
                int bookCount = 0;
                if (Books != null) Books.Clear();
                foreach (var bookItem in allBooks)
                {
                    BooksPropertyModel book = new BooksPropertyModel()
                    {
                        Author = bookItem.Author,
                        Book_Id = bookItem.Book_Id,
                        Country = bookItem.Country,
                        Book_ImageSource = bookItem.IsCoverAvailable ? bookItem.ImageLink : "PlaceHolder.png",
                        ImageLink = bookItem.ImageLink,
                        Language = bookItem.Language,
                        Link = bookItem.Link,
                        Pages = bookItem.Pages,
                        Title = bookItem.Title,
                        Year = bookItem.Year,
                    };
                    await LoaderMessage($"Added Books To View {bookCount} out of {allBooks.Count}", 100);
                    bookCount++;
                    books.Add(book);
                }
            }
            catch (Exception ex)
            {

            }
            await LoadFinalTextOfLoader();
            return books;
        }

        private async Task LoadFinalTextOfLoader()
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
            LoaderVisible = false;
        }
        private async Task LoaderMessage(string loaderText, int timeDeley, bool isStopLoader = false)
        {
            LoaderText = $"{loaderText}";
            await Task.Delay(timeDeley);
        }
        #endregion
    }
}
