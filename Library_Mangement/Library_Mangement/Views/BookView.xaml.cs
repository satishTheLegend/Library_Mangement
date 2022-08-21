using Library_Mangement.Database.Models;
using Library_Mangement.Model;
using Library_Mangement.ViewModels;
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
        #region Properties
        public readonly BookViewModel _vm;
        #endregion

        #region Constructor
        public BookView()
        {
            InitializeComponent();
            _vm = new BookViewModel();
            BindingContext = _vm;
        }
        #endregion

        #region Override
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                await _vm.LoadBooks();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}