using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblUser : BaseModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public long ID { get; set; }
        public string StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Catagories { get; set; }
        public string Phone { get; set; }
        public DateTime DOB { get; set; }
        public string UserToken { get; set; }
        public bool IsActiveUser { get; set; }
        public DateTime LoginTime { get; set; }

    }
}
