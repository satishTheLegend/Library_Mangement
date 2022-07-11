using Library_Mangement.ViewModels;
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
            VM = new SpalshViewModel();
            BindingContext = VM;
        }
        #endregion

        #region Override Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
        #endregion

    }
}