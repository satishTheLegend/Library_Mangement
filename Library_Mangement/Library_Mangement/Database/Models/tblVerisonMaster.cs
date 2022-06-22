using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblVerisonMaster : BaseModel
    {
        [PrimaryKey]
        [AutoIncrement]
        public long ID { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

    }
}
