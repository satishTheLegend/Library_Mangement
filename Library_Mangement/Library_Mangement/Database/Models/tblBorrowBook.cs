using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblBorrowBook : BaseModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public long ID { get; set; }
        public string BorrowID { get; set; }
        public long RollNo { get; set; }
        public string UserID { get; set; }
        public string BookISBN { get; set; }
        public DateTime BorrowDate { get; set; }
        public DateTime EndBorrowDate { get; set; }
        public int BorrowDays { get; set; }
        public bool IsUploaded { get; set; }
        public bool IsExpired { get; set; }
    }
}
