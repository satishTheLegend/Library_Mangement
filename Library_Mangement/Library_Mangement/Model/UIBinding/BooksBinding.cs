using Library_Mangement.Validation;
using Library_Mangement.Views.Cards;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Library_Mangement.Model.UIBinding
{
    public class BooksBinding : ValidatableBase
    {

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
        private List<BooksPropertyModel> _bookList;
        public List<BooksPropertyModel> bookList
        {
            get => _bookList;
            set
            {
                _bookList = value;
                OnPropertyChanged(nameof(_bookList));
            }
        }
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


    }
}
