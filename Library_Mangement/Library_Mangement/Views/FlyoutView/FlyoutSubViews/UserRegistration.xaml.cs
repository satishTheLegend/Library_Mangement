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
    public partial class ProfileView : ContentPage
    {
        #region Properties
        public readonly ProfileViewViewModel _vm;
        #endregion

        #region Constructor
        public ProfileView()
        {
            InitializeComponent();
            _vm = new ProfileViewViewModel();
            BindingContext = _vm;
        }
        #endregion


        protected async override void OnAppearing()
        {
            if(_vm.FieldItems == null || _vm.FieldItems?.Count < 1)
                await _vm.LoadRegistrationForm(profileStack);
            base.OnAppearing(); 
        }
    }
}