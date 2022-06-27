using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model;
using Library_Mangement.Validation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class HomeViewModel : ValidatableBase, INotifyPropertyChanged
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
                if (Books?.Count > 0)
                {
                    LottieAnimationName = "Downloading_Files.json";
                    LoaderVisible = false;
                }
                else
                {
                    LottieAnimationName = "Data_NotFound.json";
                    LoaderText = "OOPS !!!! We didnt found your book, Sorry !";
                }
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

        #region Constructor
        public HomeViewModel()
        {

        }
        #endregion

        #region Commands
        public ICommand OpenBookCommand => new Command(stack => OpenBookClicked(stack as StackLayout));
        //public ICommand SearchCommand => new Command(() => SearchClicked());
        public ICommand ViewMoreCommand => new Command(stack => ViewMoreClicked(stack as StackLayout));
        public ICommand AddToCartCommand => new Command(stack => AddToCartClicked(stack as StackLayout));
        public ICommand SmallDescCommand => new Command(stack => SmallDescClicked(stack as StackLayout));
        public ICommand LikeBookCommand => new Command(stack => LikeBookClicked(stack as StackLayout));
        public ICommand ShareBookCommand => new Command(stack => ShareBookClicked(stack as StackLayout));
        public ICommand LongDescriptionCommand => new Command((desc) => OpenLongDescriptionClicked(desc));
        public ICommand ShortDescriptionCommand => new Command((desc) => OpenShortDescriptionClicked(desc));
        #endregion

        #region Event Handlers
        private void OpenBookClicked(StackLayout stack)
        {
            //
        }
        private void ViewMoreClicked(StackLayout stack)
        {
            try
            {
                if (stack.BindingContext is BooksPropertyModel data)
                {
                    var removeShowMore = Books.Where(x => x.ISBN != data.ISBN && x.ViewMoreButtons).ToList();
                    UpdateBookItemIndex(removeShowMore);
                    var showMoreBooks = Books.Where(x => x.ISBN == data.ISBN).ToList();
                    if (!showMoreBooks[0].ViewMoreButtons)
                    {
                        UpdateBookItemIndex(showMoreBooks, true);
                    }
                    else
                    {
                        UpdateBookItemIndex(showMoreBooks);
                    }

                }
            }
            catch (Exception ex)
            {

            }
        }
        private async void AddToCartClicked(StackLayout stackLayout)
        {
            if (stackLayout.BindingContext is BooksPropertyModel data)
            {
                await App.Current.MainPage.DisplayAlert("Attention", "This feature is under development will get back to you in some day !!!!!", "ok", "cancel");
            }
        }
        private async void SmallDescClicked(StackLayout stackLayout)
        {
            if (stackLayout.BindingContext is BooksPropertyModel data)
            {
                await App.Current.MainPage.DisplayAlert(data.Title, data.ShortDescription, "ok");
            }
        }
        private async void LikeBookClicked(StackLayout stackLayout)
        {
            if (stackLayout.BindingContext is BooksPropertyModel data)
            {
                await App.Current.MainPage.DisplayAlert("Like", $"Thank you for feedback for {data.Title}", "ok");
            }
        }
        private async void ShareBookClicked(StackLayout stackLayout)
        {
            if (stackLayout.BindingContext is BooksPropertyModel data)
            {
                bool res = await App.Current.MainPage.DisplayAlert("Share Book", data.Title, "ok", "cancel");
                if (res)
                {
                    await Share.RequestAsync(new ShareTextRequest
                    {
                        Text = data.Title,
                        Title = "Share Title"
                    });
                }
            }
        }
        private void UpdateBookItemIndex(List<BooksPropertyModel> books, bool isVisible = false)
        {
            if (books?.Count > 0)
            {
                foreach (var book in books)
                {
                    var index = Books.IndexOf(book);
                    if (index > -1)
                    {
                        Books.RemoveAt(index);
                        book.ViewMoreButtons = isVisible;
                        Books.Insert(index, book);
                    }
                }
            }
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
                    await LoaderMessage($"Fetched {allBooks.Count} Books From Database", 1300);
                    await LoaderMessage($"Arrenging Books Please Wait ....", 1300);
                    bookList = await LoadBooksFromTable(allBooks);
                    if (bookList?.Count > 0)
                        Books = new ObservableCollection<BooksPropertyModel>(bookList);
                }
            }
            catch (Exception ex)
            {

            }
        }
        public void SearchClicked()
        {
            if (!string.IsNullOrEmpty(SearchText))
            {
                var bookItems = bookList.Where(x => (!string.IsNullOrEmpty(x.Title) && x.Title.Contains(SearchText)) || (!string.IsNullOrEmpty(x.Author) && x.Author.Contains(SearchText)) || (!string.IsNullOrEmpty(x.ISBN) && x.ISBN.Contains(SearchText)) || (!string.IsNullOrEmpty(x.Categories) && x.Categories.Contains(SearchText))).ToList();
                Books.Clear();
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
                Books.Clear();
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
                    await LoaderMessage($"Added Books To View {bookCount} out of {allBooks.Count}", 50);
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
            if (timeDeley > 0 && AppConfig.isAwaitTimeNeeds)
            {
                await Task.Delay(timeDeley);
            }
        }
        #endregion
    }
}
