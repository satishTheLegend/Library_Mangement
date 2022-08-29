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
    public partial class CategoryView : ContentPage
    {
        #region Properties
        public readonly CategoryViewModel _vm;
        #endregion

        #region Constructor
        public CategoryView()
        {
            _vm = new CategoryViewModel();
            InitializeComponent();
            BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected async override void OnAppearing()
        {
            await _vm.ExploreBooks();
            base.OnAppearing();
        }
        #endregion
    }
}