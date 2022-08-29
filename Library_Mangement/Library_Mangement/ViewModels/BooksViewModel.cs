using Library_Mangement.Database.Models;
using Library_Mangement.Model;
using Library_Mangement.Validation;
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
    public class BooksViewModel : ValidatableBase
    {
        #region Properties
        List<BooksPropertyModel> bookList = null;
        private ObservableCollection<BooksPropertyModel> _books;
        public ObservableCollection<BooksPropertyModel> Books
        {
            get => _books;
            set
            {
                _books = value;
                LoaderVisible = true;
                if (Books?.Count > 0 && SearchText?.Length > 0)
                {
                    LottieAnimationName = "Downloading_Files.json";
                    LoaderVisible = false;
                }
                else if (SearchText?.Length > 0)
                {
                    LottieAnimationName = "Data_NotFound.json";
                    LoaderText = "OOPS !!!! We didnt found your book, Sorry !";
                }
                OnPropertyChanged(nameof(Books));
            }
        }

        private ObservableCollection<BooksGroup> _booksGroup;
        public ObservableCollection<BooksGroup> BooksGroup
        {
            get => _booksGroup;
            set
            {
                _booksGroup = value;
                OnPropertyChanged(nameof(_booksGroup));
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

        private double _loaderPercent;
        public double LoaderPercent
        {
            get => _loaderPercent;
            set
            {
                _loaderPercent = value;
                OnPropertyChanged(nameof(_loaderPercent));
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

        #region Contructor
        public BooksViewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand OpenBookCommand => new Command(frame => OpenBookClicked(frame as Frame));
        public ICommand SearchCommand => new Command(() => SearchClicked());
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
                //await App.Current.MainPage.Navigation.PushAsync(new BookView(book));
                LoaderVisible = false;
            }
        }
        #endregion

        #region Public Methods
        public async Task LoadBooksInfo(string catagoryBook)
        {
            try
            {
                LoaderVisible = true;
                await LoaderMessage($"Getting Books From Database", 1300);
                List<tblBook> allBooks = null;
                if (!string.IsNullOrEmpty(catagoryBook))
                {
                    allBooks = await App.Database.Book.GetBooksByCatagory(catagoryBook); 
                }
                else
                {
                    allBooks = await App.Database.Book.GetDataAsync();
                }
                if (allBooks?.Count > 0)
                {
                    await LoaderMessage($"Fetched {allBooks.Count} Books From Database", 1300);
                    await LoaderMessage($"Arrenging Books Please Wait ....", 1300);
                    bookList = await LoadBooksFromTable(allBooks);
                    LoaderVisible = false;
                }
                else
                {
                    LottieAnimationName = "Data_NotFound.json";
                    LoaderText = "OOPS !!!! We didnt found your books, We are working on it !";
                }
            }
            catch (Exception ex)
            {

            }
        }
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
                        Books = new ObservableCollection<BooksPropertyModel>(bookItems);
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
                    Books = new ObservableCollection<BooksPropertyModel>(bookList);
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
        private async Task<List<BooksPropertyModel>> LoadBooksFromTable(List<tblBook> allBooks)
        {
            List<BooksPropertyModel> books = new List<BooksPropertyModel>();
            try
            {
                int bookCount = 0;
                if (Books != null) Books.Clear(); else Books = new ObservableCollection<BooksPropertyModel>();

                foreach (var bookItem in allBooks)
                {
                    BooksPropertyModel book = new BooksPropertyModel()
                    {
                        ISBN = bookItem.ISBN,
                        Catagory = bookItem.Categories,
                        Auther = bookItem.Authors,
                        PublishYear = bookItem.PublishedDate,
                        Book_ImageSource = File.Exists(bookItem.PngFilePath) ? bookItem.PngFilePath : "No_Thumb.png",
                        IsCoverAvailable = bookItem.IsCoverAvailable,
                        PageCount = bookItem.PageCount,
                        Title = bookItem.Title,
                    };
                    SetProgressBarValue(bookCount, allBooks.Count);
                    if (bookCount == allBooks.Count - 1)
                    {
                        await LoaderMessage($"Loading...", 0);

                    }
                    else
                    {
                        await LoaderMessage($"Added Books To View {bookCount} out of {allBooks.Count}", 10);
                    }
                    bookCount++;
                    Books.Add(book);
                    books.Add(book);

                }
            }
            catch (Exception ex)
            {

            }
            return books;
        }

        private void SetProgressBarValue(int i, int count)
        {
            double runtimePercent = 0.0;
            double percentValue = count / 100;
            if (percentValue > 0)
            {
                runtimePercent = i / percentValue;
            }
            LoaderPercent = runtimePercent;
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
            if (timeDeley > 0)
            {
                await Task.Delay(timeDeley);
            }
        }
        #endregion
    }
}
