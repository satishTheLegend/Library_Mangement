using Library_Mangement.Database.Models;
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
    public partial class OpenBookView : ContentPage
    {
        #region Properties
        public readonly OpenBookViewModel _vm;
        public tblBook _bookData;
        #endregion

        #region Constructor
        public OpenBookView(tblBook bookData)
        {
            _bookData = bookData;
            InitializeComponent();
            _vm = new OpenBookViewModel();
            BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadBookDataAsync(_bookData);
        }
        #endregion
    }
}