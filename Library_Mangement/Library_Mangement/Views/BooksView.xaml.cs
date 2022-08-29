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
                _vm.LoaderVisible = true;
                if (Application.Current.MainPage.Navigation.NavigationStack != null && Application.Current.MainPage.Navigation.NavigationStack.Count > 1)
                {
                    var existingPages = Application.Current.MainPage.Navigation.NavigationStack.ToList();
                    foreach (var pageItem in existingPages)
                    {
                        bool flag = (pageItem is BooksView);
                        if (!flag)
                        {
                            Application.Current.MainPage.Navigation.RemovePage(pageItem);
                        }
                    }
                }
                _vm.LoaderVisible = false;
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