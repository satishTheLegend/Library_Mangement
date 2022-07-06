using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse
{
    //public class BooksJsonData
    //{
    //    public string author { get; set; }
    //    public string country { get; set; }
    //    public string imageLink { get; set; }
    //    public string language { get; set; }
    //    public string link { get; set; }
    //    public int pages { get; set; }
    //    public string Title { get; set; }
    //    public int year { get; set; }
    //}

    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<Root>>(myJsonResponse);
    public class PublishedDate
    {
        [JsonProperty("$date")]
        public DateTime Date { get; set; }
    }

    public class BooksJsonData
    {
        public string FileName { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public int PageCount { get; set; }
        public string PdfFileName { get; set; }
        public int FileLength { get; set; }
        public string AuthorName { get; set; }
        public string ISBN { get; set; }
        public string PublishDate { get; set; }
        public string Catagories { get; set; }
        public string PDFFileLink { get; set; }
        public string PNGFileLink { get; set; }
        public string Status { get; set; }
        public string FilePath { get; set; }
    }
}
