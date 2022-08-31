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
    public partial class MyCartView : ContentPage
    {
        #region Properties
        public readonly MyCartViewModel _vm;
        #endregion

        #region Constructor
        public MyCartView()
        {
            InitializeComponent();
            _vm = new MyCartViewModel();
            BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected async override void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadCartBooks();
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