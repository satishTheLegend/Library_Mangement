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
        public int CodeId { get; set; }
        public string CodeName { get; set; }
        public string CodeText { get; set; }
        public string CodeValue { get; set; }
        public int CodeSeq { get; set; }
        public string GroupName { get; set; }
        public string CodeDesc { get; set; }
        public string status { get; set; }
    }
}
