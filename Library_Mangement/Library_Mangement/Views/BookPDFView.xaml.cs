﻿using Acr.UserDialogs;
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
        //public readonly BookPDFViewModel _vm;
        public string _pdfFilePath = string.Empty;
        #endregion

        #region Constructor
        public BookPDFView(string pdfFilePath, string bookName)
        {
            InitializeComponent();
            _pdfFilePath = pdfFilePath;
            this.Title = bookName;
            //_vm = new BookPDFViewModel();
            //BindingContext = _vm;
        }
        #endregion

        #region Override Methods
        protected async override void OnAppearing()
        {
            UserDialogs.Instance.ShowLoading("Opening File");
            base.OnAppearing();
            _pdfFilePath = _pdfFilePath.Replace("https://drive.google.com/u/0/uc?id=", "https://drive.google.com/file/d/");
            _pdfFilePath = _pdfFilePath.Replace("&export=download", "/view");
            pdfWebView.Source = _pdfFilePath;
            //await _vm.LoadPdfFromPath(_pdfFilePath);
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