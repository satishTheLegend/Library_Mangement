using Acr.UserDialogs;
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
    public partial class BookPDFView : ContentPage
    {

        #region Properties
        public readonly BookPDFViewModel _vm;
        public string _pdfFilePath = string.Empty;
        #endregion

        #region Constructor
        public BookPDFView(string pdfFilePath)
        {
            _pdfFilePath = pdfFilePath;
            InitializeComponent();
            _vm = new BookPDFViewModel();
            BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected async override void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("Opening File");
            base.OnAppearing();
            await _vm.LoadPdfFromPath(_pdfFilePath);
            UserDialogs.Instance.HideLoading();
        }
        #endregion

        #region Event Handlers

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        #endregion

    }
}