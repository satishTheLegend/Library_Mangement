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
<<<<<<< HEAD:Library_Mangement/Library_Mangement/Views/UserRegistration.xaml.cs
    public partial class UserRegistration : ContentPage
    {
        #region Properites
        private readonly UserRegistrationViewModel _vm;
        #endregion

        #region Constructor
        public UserRegistration()
=======
    public partial class CategoryView : ContentPage
    {
        #region Properties
        public readonly CategoryViewModel _vm;
        #endregion

        #region Constructor
        public CategoryView()
>>>>>>> Updated Data with Categories:Library_Mangement/Library_Mangement/Views/FlyoutView/FlyoutSubViews/CategoryView.xaml.cs
        {
            _vm = new CategoryViewModel();
            InitializeComponent();
<<<<<<< HEAD:Library_Mangement/Library_Mangement/Views/UserRegistration.xaml.cs
            _vm = new UserRegistrationViewModel(MainStackScroll, MainStack);
=======
>>>>>>> Updated Data with Categories:Library_Mangement/Library_Mangement/Views/FlyoutView/FlyoutSubViews/CategoryView.xaml.cs
            BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected async override void OnAppearing()
        {
<<<<<<< HEAD:Library_Mangement/Library_Mangement/Views/UserRegistration.xaml.cs
            await _vm.LoadData();
            base.OnAppearing();
        }
        #endregion

        #region Event Handlers

        #endregion

=======
            await _vm.ExploreBooks();
            base.OnAppearing();
        }
        #endregion
>>>>>>> Updated Data with Categories:Library_Mangement/Library_Mangement/Views/FlyoutView/FlyoutSubViews/CategoryView.xaml.cs
    }
}