using Library_Mangement.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Database.Models
{
    public class tblSettings : BaseModel
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
