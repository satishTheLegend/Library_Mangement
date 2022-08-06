using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse.GETModels
{
    public class BooksMasterList
    {
        public int id { get; set; }
        public string title { get; set; }
        public string isbn { get; set; }
        public int pageCount { get; set; }
        public string publishedDate { get; set; }
        public string pngLink { get; set; }
        public string pngName { get; set; }
        public string pdfLink { get; set; }
        public string pdfName { get; set; }
        public string status { get; set; }
        public string authors { get; set; }
        public string categories { get; set; }
        public bool isCoverAvailable { get; set; }
    }

    public class BooksJsonMasterData
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public List<BooksMasterList> data { get; set; }
    }
}
