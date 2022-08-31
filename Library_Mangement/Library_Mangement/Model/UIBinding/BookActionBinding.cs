using Library_Mangement.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Library_Mangement.Model.UIBinding
{
    public class BookActionBinding : ValidatableBase
    {
        private ImageSource _bookImage;
        public ImageSource BookImage
        {
            get => _bookImage;
            set
            {
                _bookImage = value;
                OnPropertyChanged(nameof(BookImage));
            }
        }

        private string _bookTitle;
        public string BookTitle
        {
            get => _bookTitle;
            set
            {
                _bookTitle = value;
                OnPropertyChanged(nameof(BookTitle));
            }
        }

        private string _bookAuther;
        public string BookAuther
        {
            get => _bookAuther;
            set
            {
                _bookAuther = value;
                OnPropertyChanged(nameof(BookAuther));
            }
        }

        private string _bookIssueDate;
        public string BookIssueDate
        {
            get => _bookIssueDate;
            set
            {
                _bookIssueDate = value;
                OnPropertyChanged(nameof(BookIssueDate));
            }
        }

        private string _bookReturnDate;
        public string BookReturnDate
        {
            get => _bookReturnDate;
            set
            {
                _bookReturnDate = value;
                OnPropertyChanged(nameof(BookReturnDate));
            }
        }

        private string _bookCategory;
        public string BookCategory
        {
            get => _bookCategory;
            set
            {
                _bookCategory = value;
                OnPropertyChanged(nameof(BookCategory));
            }
        }

        private string _bookIssueDays;
        public string BookIssueDays
        {
            get => _bookIssueDays;
            set
            {
                _bookIssueDays = value;
                OnPropertyChanged(nameof(BookIssueDays));
            }
        }

        private string _bookPublishYear;
        public string BookPublishYear
        {
            get => _bookPublishYear;
            set
            {
                _bookPublishYear = value;
                OnPropertyChanged(nameof(BookPublishYear));
            }
        }

        private string _bookISBN;
        public string BookISBN
        {
            get => _bookISBN;
            set
            {
                _bookISBN = value;
                OnPropertyChanged(nameof(BookISBN));
            }
        }


        private string _bookActionId;
        public string BookActionId
        {
            get => _bookActionId;
            set
            {
                _bookActionId = value;
                OnPropertyChanged(nameof(BookActionId));
            }
        }
    }
}
