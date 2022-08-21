using Library_Mangement.Database.Models;
using Library_Mangement.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Library_Mangement.Views.Cards
{
    public class BooksGroup : ObservableCollection<BooksPropertyModel>
    {
        public string _bookCatagory { get; private set; }
        public ObservableCollection<BooksPropertyModel> _books { get; private set; }
        public BooksGroup(string bookCatagory, ObservableCollection<BooksPropertyModel> books) : base(books)
        {
            _bookCatagory = bookCatagory;
            _books = books;
        }
    }
}
