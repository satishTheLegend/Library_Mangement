using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblCodesMaster : BaseModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        public string CodeId { get; set; }
        public string CodeText { get; set; }
        public string CodeValue { get; set; }
    }
}
