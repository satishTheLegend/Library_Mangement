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
    public partial class CatagoryChips : ContentPage
    {
        #region Properties
        public readonly CatagoryChipsViewModel _vm;
        #endregion

        #region Constructor
        public CatagoryChips()
        {
            InitializeComponent();
            _vm = new CatagoryChipsViewModel();
            BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected async override void OnAppearing()
        {
            await _vm.LoadCatagories();
            base.OnAppearing();
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