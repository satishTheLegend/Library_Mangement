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
    public partial class UserRegistration : ContentPage
    {
        #region Properites
        private readonly UserRegistrationViewModel _vm;
        #endregion

        #region Constructor
        public UserRegistration()
        {
            _vm = new UserRegistrationViewModel(MainStackLayout);
            BindingContext = _vm;
            InitializeComponent();
        }
        #endregion

        #region Override Methods
        protected async override void OnAppearing()
        {
            await _vm.LoadData();
            base.OnAppearing();
        }
        #endregion

        #region Event Handlers

        #endregion

    }
}