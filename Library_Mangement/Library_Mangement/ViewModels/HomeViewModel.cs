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
        public ICommand LongDescriptionCommand => new Command((desc) => OpenLongDescriptionClicked(desc));

        public ICommand ShortDescriptionCommand => new Command((desc) => OpenShortDescriptionClicked(desc));

        #endregion

        #region Event Handlers
        private void OpenBookClicked()
        {
            //
        }

        private void OpenLongDescriptionClicked(object data)
        {
            if (data != null)
            {

            }
        }
        private void OpenShortDescriptionClicked(object data)
        {
            if (data != null)
            {

            }
        }

        #endregion

        #region Public Methods
        public async Task LoadBooksInfo()
        {
            try
            {
                LoaderVisible = true;
                await LoaderMessage($"Getting Books From Database", 1300);
                var allBooks = await App.Database.Book.GetDataAsync();
                if (allBooks?.Count > 0)
                {
                    Books = new ObservableCollection<BooksPropertyModel>();
                    await LoaderMessage($"Fetched {allBooks.Count} Books From Database", 1300);
                    await LoaderMessage($"Arrenging Books Please Wait ....", 1300);
                    List<BooksPropertyModel> bookList = await LoadBooksFromTable(allBooks);
                    if (bookList?.Count > 0)
                    {
                        foreach (var item in bookList)
                        {
                            Books.Add(item);
                        }
                    }
                        //Books = new ObservableCollection<BooksPropertyModel>(bookList);
                }
            }
            catch (Exception ex)
            {

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
                        Author = bookItem.Authors,
                        ISBN = bookItem.ISBN,
                        ShortDescription = bookItem.ShortDescription,
                        Book_ImageSource = bookItem.IsCoverAvailable ? bookItem.FilePath : "PlaceHolder.png",
                        LongDescription = bookItem.LongDescription,
                        Categories = bookItem.Categories,
                        PageCount = bookItem.PageCount,
                        Title = bookItem.Title,
                        PublishedDate = bookItem.PublishedDate.ToString("MM/dd/yyyy"),
                    };
                    await LoaderMessage($"Added Books To View {bookCount} out of {allBooks.Count}",50);
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
            LoaderVisible = false;
        }
        private async Task LoaderMessage(string loaderText, int timeDeley, bool isStopLoader = false)
        {
            LoaderText = $"{loaderText}";
            if(timeDeley > 0)
            {
                await Task.Delay(timeDeley);
            }
        }
        #endregion
    }
}
