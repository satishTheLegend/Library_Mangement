using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse.GETModels
{
    public class DynFieldsData
    {
        public int FieldId { get; set; }
        public bool Required { get; set; }
        public string Validation { get; set; }
        public string ValidationMsg { get; set; }
        public int Sequence { get; set; }
        public string GroupName { get; set; }
        public string KeyboardType { get; set; }
        public string FieldName { get; set; }
        public string ControlType { get; set; }
        public string PageName { get; set; }
        public List<ListValue> listValues { get; set; }

        public DynFieldsData()
        {
            listValues = new List<ListValue>();
        }
    }

    public class ListValue
    {
        public string type { get; set; }
        public string next { get; set; }
        public string value { get; set; }
    }

    public class OCLMDynamicFields
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public List<DynFieldsData> data { get; set; }
    }
}
