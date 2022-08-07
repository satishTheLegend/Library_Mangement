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
    public partial class Dashboard : ContentPage
    {
        #region Properties
        readonly DashboardViewModel _vm;
        #endregion

        #region Constructor
        public Dashboard()
        {
            try
            {
                InitializeComponent();
                _vm = new DashboardViewModel();
                BindingContext = _vm;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Override Methods
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _vm.LoadBooksInfo_Updated();
        }
        #endregion
    }
}