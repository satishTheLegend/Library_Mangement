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
        public string KeyName { get; set; }
        public string Link { get; set; }
        public string Verison { get; set; }
        public bool Active { get; set; }
        public bool IsRecordSaveToDB { get; set; }
        public string FilePath { get; set; }
        public string DirectoryName { get; set; }
        public string FileExtention { get; set; }
        public string FileName { get; set; }
        public string Value { get; set; }
        public bool IsRecordSaved { get; set; }

    }
}
