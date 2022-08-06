using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse.GETModels
{
    internal class OCLMCodesMaster
    {
    }
    public class CodesData
    {
        public int CodeId { get; set; }
        public string CodeName { get; set; }
        public string CodeText { get; set; }
        public string CodeValue { get; set; }
        public int CodeSeq { get; set; }
        public string GroupName { get; set; }
        public string CodeDesc { get; set; }
        public string status { get; set; }
    }

    public class LibraryCodesMaster
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public List<CodesData> data { get; set; }
    }
}
