using Library_Mangement.Animations;
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
    public partial class LoginView1 : ContentPage
    {
        public LoginView1()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Task.Run(async () =>
            {
                await ViewAnimations.FadeAnimY(Logo);
                await ViewAnimations.FadeAnimY(EmailPancake);
                await ViewAnimations.FadeAnimY(PassPancake);
                await ViewAnimations.FadeAnimY(LoginButton);

            });
        }
        protected void Back(object s, EventArgs e)
        {
            Navigation.PopAsync();
        }
        protected void Login(object s, EventArgs e)
        {
            //Navigation.PopToRootAsync();
            Navigation.PushAsync(new LoginView2());
        }
    }
}