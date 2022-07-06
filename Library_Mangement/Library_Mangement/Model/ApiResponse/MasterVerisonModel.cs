using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse
{
    public class MasterVerisonModel
    {
        public string KeyName { get; set; }
        public string Link { get; set; }
        public string Verison { get; set; }
        public bool Active { get; set; }
        public bool IsRecordSaveToDB { get; set; }
        public string DirectoryName { get; set; }
        public string FileExtention { get; set; }
        public string FileName { get; set; }
        public string Value { get; set; }
    }

}
