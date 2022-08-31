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
        public long RollNo { get; set; }
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string CollageName { get; set; }
        public string Education { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Catagories { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public string ProfilePicUrl { get; set; }
        public string ProfilePicPath { get; set; }
        public DateTime DOB { get; set; }
        public string UserToken { get; set; }
        public bool IsActiveUser { get; set; }
        public DateTime LoginTime { get; set; }

    }
}
