using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblLogs : BaseModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public DateTime Datetime { get; set; }
        public string LogType { get; set; }
        public string ModuleName { get; set; }
        public string Description { get; set; }
        public string UserInfo { get; set; }
    }
}
