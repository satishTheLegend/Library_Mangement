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
    public partial class BooksView : ContentPage
    {
        #region Properties
        public readonly BooksViewModel _vm;
        public string catagoryBook = string.Empty;
        #endregion

        #region Constructor
        public BooksView(string CatagoryBook = null)
        {
            catagoryBook = CatagoryBook;
            InitializeComponent();
            _vm = new BooksViewModel();
            BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                if (_vm.Books == null)
                {
                    await _vm.LoadBooksInfo(catagoryBook);
                }
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Event Handler
        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(_vm.SearchText))
            {
                _vm.SearchClicked();
            }
        }
        #endregion


    }
}