using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblBook : BaseModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public long Id { get; set; }
        public string Author { get; set; }
        public string Country { get; set; }
        public string ImageLink { get; set; }
        public string Language { get; set; }
        public string Link { get; set; }
        public int Pages { get; set; }
        public string Title { get; set; }
        public int Year { get; set; }
        public bool IsCoverAvailable { get; set; }
    }
}
