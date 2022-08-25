﻿using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model;
using Library_Mangement.Services;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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
        public ICommand OpenBookCommand => new Command(frame => OpenBookClicked(frame as Frame));
        public ICommand SearchCommand => new Command(() => SearchClicked());
        #endregion

        #region Event Handlers
        private async void OpenBookClicked(Frame frame)
        {
            var bookData = frame.BindingContext as BooksPropertyModel;
            if(bookData != null)
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
                    //await LoadFinalTextOfLoader();
                    LoaderVisible = false;
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
                if (Books != null) Books.Clear();
                               
                foreach (var bookItem in allBooks)
                {
                    if (!File.Exists(bookItem.PngFilePath))
                    {
                        await RestService.DownloadFileFromURIAndSaveIt(bookItem.PngFilePath, bookItem.PngFilePath);
                    }
                    BooksPropertyModel book = new BooksPropertyModel()
                    {
                        ISBN = bookItem.ISBN,
                        Book_ImageSource = bookItem.IsCoverAvailable ? bookItem.PngFilePath : "PlaceHolder.png",
                        IsCoverAvailable = bookItem.IsCoverAvailable,
                        PageCount = bookItem.PageCount,
                        Title = bookItem.Title,
                    };
                    if(bookCount == allBooks.Count-1)
                    {
                        await LoaderMessage($"Loading...", 0);
                    }
                    else
                    {
                        await LoaderMessage($"Added Books To View {bookCount} out of {allBooks.Count}", 10);
                    }
                    bookCount++;
                    books.Add(book);
                }
            }
            catch (Exception ex)
            {

            }
            return await Task.FromResult(books);
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
