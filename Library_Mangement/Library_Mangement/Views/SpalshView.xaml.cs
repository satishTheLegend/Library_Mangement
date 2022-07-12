using Library_Mangement.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SpalshView : ContentPage
    {
        #region Properties
        public readonly SpalshViewModel VM;
        #endregion

        #region Constructor
        public SpalshView()
        {
            InitializeComponent();
            VM = new SpalshViewModel(SplashPage);
            BindingContext = VM;
        }
        #endregion

        #region Override Methods
        protected async override void OnAppearing()
        {
            var isRecordAdded = await App.Database.Settings.FindByKeyAsync("splash");
            try
            {
                if (isRecordAdded != null && !string.IsNullOrEmpty(isRecordAdded.Value) && Convert.ToBoolean(Convert.ToInt32(isRecordAdded.Value)))
                {
                    await App.Current.MainPage.Navigation.PushAsync(new LandingView(true));
                }
                else
                {
                    VM.NextClicked(true);
                }
                base.OnAppearing();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}