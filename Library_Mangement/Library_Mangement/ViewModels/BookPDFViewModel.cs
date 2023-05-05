using Library_Mangement.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.ViewModels
{
    public class BookPDFViewModel : ValidatableBase
    {

        #region Properties

        private Stream m_pdfDocumentStream;
        public Stream PdfDocumentStream
        {
            get
            {
                return m_pdfDocumentStream;
            }
            set
            {
                m_pdfDocumentStream = value;
                OnPropertyChanged("PdfDocumentStream");
            }
        }



        #endregion

        #region Constructor
        public BookPDFViewModel()
        {

        }
        #endregion

        #region Commands

        #endregion

        #region Event Handlers

        #endregion

        #region Public Methods
        public async Task LoadPdfFromPath(string pdfFilePath)
        {
            pdfFilePath = pdfFilePath.Replace("https://drive.google.com/u/0/uc?id=", "https://drive.google.com/file/d/");
            pdfFilePath = pdfFilePath.Replace("&export=download", "/view");
            PdfDocumentStream = File.OpenRead(pdfFilePath);
            //if (File.Exists(pdfFilePath))
            //{
                
            //}
            //else
            //{
            //    await App.Current.MainPage.DisplayAlert("Error", "File not found", "OK");
            //    await App.Current.MainPage.Navigation.PopAsync();
            //}
            await Task.FromResult(Task.CompletedTask);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
