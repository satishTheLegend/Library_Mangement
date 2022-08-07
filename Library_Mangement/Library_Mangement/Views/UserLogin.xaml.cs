using Library_Mangement.Animations;
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

        #region Override Methods
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            await Task.Run(async () =>
            {
                await ViewAnimations.FadeAnimY(Logo);
                await ViewAnimations.FadeAnimY(BgImage);
                await ViewAnimations.FadeAnimY(RemMe);
                await ViewAnimations.FadeAnimY(button);
            });

#if DEBUG
            userName.Text = "a@p.com";
            password.Text = "ap";
#endif
        }
        #endregion

        #region Debug Condition
        #endregion

    }
}