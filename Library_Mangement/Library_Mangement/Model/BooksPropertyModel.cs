using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Library_Mangement.Model
{
    public class BooksPropertyModel
    {
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int PageCount { get; set; }
        public ImageSource Book_ImageSource { get; set; }
        public bool IsCoverAvailable { get; set; }
    }
}
