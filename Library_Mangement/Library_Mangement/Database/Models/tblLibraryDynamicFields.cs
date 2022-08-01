using Library_Mangement.Model;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblLibraryDynamicFields : BaseModel
    {
        [PrimaryKey] [AutoIncrement]
        public int FieldId { get; set; }
        public bool Required { get; set; }
        public string Validation { get; set; }
        public string ValidationMsg { get; set; }
        public int Sequence { get; set; }
        public string FieldName { get; set; }
        public string ControlType { get; set; }
        public string PageName { get; set; }
        public string ListValues { get; set; }
    }
}
