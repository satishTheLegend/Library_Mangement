using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblAddToCart : BaseModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public long ID { get; set; }
        public string CartID { get; set; }
        public long RollNo { get; set; }
        public string UserID { get; set; }
        public string BookISBN { get; set; }
        public string Type { get; set; }
        public DateTime AddedtoCartDate { get; set; }
        public DateTime RemoveCartDate { get; set; }
        public bool IsUploaded { get; set; }
        public bool IsRemovedFCart { get; set; }
        public bool IsRemovedFWL { get; set; }
    }
}
