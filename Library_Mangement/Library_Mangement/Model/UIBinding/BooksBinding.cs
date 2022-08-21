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

        private ObservableCollection<BooksGroup> _books;
        public ObservableCollection<BooksGroup> Books
        {
            get => _books;
            set
            {
                _books = value;
                OnPropertyChanged(nameof(_books));
            }
        }
        private ObservableCollection<BooksPropertyModel> _booksData;
        public ObservableCollection<BooksPropertyModel> BooksData
        {
            get => _booksData;
            set
            {
                _booksData = value;
                OnPropertyChanged(nameof(_booksData));
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
