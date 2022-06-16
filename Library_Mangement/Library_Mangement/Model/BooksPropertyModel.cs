using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Library_Mangement.Model
{
    public class BooksPropertyModel
    {
        public string Book_Id { get; set; }
        public ImageSource Book_ImageSource { get; set; }
        public string Author { get; set; }
        public string Country { get; set; }
        public string ImageLink { get; set; }
        public string Language { get; set; }
        public string Link { get; set; }
        public int Pages { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public bool IsCoverAvailable { get; set; }
    }
}
