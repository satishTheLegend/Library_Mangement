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
    public partial class UserLogin : ContentPage
    {
        #region Properties
        readonly UserLoginVIewModel _vm;
        #endregion

        #region Constructor
        public UserLogin()
        {
            InitializeComponent();
            _vm = new UserLoginVIewModel();
            BindingContext = _vm;
        }
        #endregion

    }
}