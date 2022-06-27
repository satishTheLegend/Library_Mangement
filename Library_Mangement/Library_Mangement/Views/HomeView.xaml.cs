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
    public partial class HomeView : ContentPage
    {
        #region Properties
        readonly HomeViewModel _vm;
        #endregion

        #region Constructor
        public HomeView()
        {
            InitializeComponent();
            _vm = new HomeViewModel();
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
                    await _vm.LoadBooksInfo();
                }
                _vm.LoaderVisible = true;
                if (Application.Current.MainPage.Navigation.NavigationStack != null && Application.Current.MainPage.Navigation.NavigationStack.Count > 1)
                {
                    var existingPages = Application.Current.MainPage.Navigation.NavigationStack.ToList();
                    foreach (var pageItem in existingPages)
                    {
                        bool flag = (pageItem is HomeView);
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

        #region Event Handlers

        #endregion

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            _vm.SearchClicked();
        }
    }
}