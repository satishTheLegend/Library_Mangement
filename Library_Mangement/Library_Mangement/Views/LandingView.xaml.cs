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
    public partial class LandingView : ContentPage
    {
        #region Properties
        public readonly LandingViewModel _vm;
        #endregion

        #region Constructor
        public LandingView(bool LogOutUser = false)
        {
            InitializeComponent();
            _vm = new LandingViewModel();
            BindingContext = _vm;
        }
        #endregion

        #region override methods 
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () =>
            {
                await ViewAnimations.FadeAnimY(Logo);
                await ViewAnimations.FadeAnimY(FaceButton);
                await ViewAnimations.FadeAnimY(LoginButton);
                await ViewAnimations.FadeAnimY(SignupButton);
            });
            await _vm.DownloadMasterData();
        }
        protected void Back(object s, EventArgs e)
        {
            Navigation.PopAsync();
        }
        protected void Login(object s, EventArgs e)
        {
            Navigation.PushAsync(new UserLogin());
        }
        #endregion
    }
}