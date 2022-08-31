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
            if(File.Exists(pdfFilePath))
            {
                PdfDocumentStream = File.OpenRead(pdfFilePath);
            }
            await Task.FromResult(Task.CompletedTask);
        }
        #endregion

        #region Private Methods
        #endregion
    }
}
