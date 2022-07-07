using Library_Mangement.Database.Models;
using Library_Mangement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookView : ContentPage
    {
        public BookView(tblBook activeBookData)
        {
            InitializeComponent();
            if(activeBookData != null)
            {
                bookView.Title = !string.IsNullOrEmpty(activeBookData.Title) ? activeBookData.Title : "Book Details";
            }
        }
    }
}