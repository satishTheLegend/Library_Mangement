using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    //public class tblBook : BaseModel
    //{
    //    [PrimaryKey]
    //    [AutoIncrement]
    //    public long Id { get; set; }
    //    public string Book_Id { get; set; } = Guid.NewGuid().ToString("N").Substring(0, 10);
    //    public string Author { get; set; }
    //    public string Country { get; set; }
    //    public string ImageLink { get; set; }
    //    public string Language { get; set; }
    //    public string Link { get; set; }
    //    public int Pages { get; set; }
    //    public string Title { get; set; }
    //    public int Year { get; set; }
    //    public bool IsCoverAvailable { get; set; }
    //}

    public class tblBook : BaseModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int PageCount { get; set; }
        public string PublishedDate { get; set; }
        public string PngLink { get; set; }
        public string PngName { get; set; }
        public string PdfLink { get; set; }
        public string PdfName { get; set; }
        public string Status { get; set; }
        public string Authors { get; set; }
        public string Categories { get; set; }
        public string PngFilePath { get; set; }
        public string PdfFilePath { get; set; }
        public bool IsCoverAvailable { get; set; }
    }


}
