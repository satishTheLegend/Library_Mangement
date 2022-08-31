using Library_Mangement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Views.FlyoutView.FlyoutSubViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyBooksView : ContentPage
    {
        #region Properties
        public readonly MyBooksViewModel _vm;
        #endregion

        #region Constructor
        public MyBooksView()
        {
            InitializeComponent();
            _vm = new MyBooksViewModel();
            BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadMyBooks();
        }
        #endregion

        #region Event Handlers

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion
    }
}