using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse
{
    // LibraryDynamicFields myDeserializedClass = JsonConvert.DeserializeObject<LibraryDynamicFields>(myJsonResponse);
    public class FieldsData
    {
        public bool Required { get; set; }
        public string Validation { get; set; }
        public string ValidationMsg { get; set; }
        public int Sequence { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public string ControlType { get; set; }
        public string PageName { get; set; }
        public List<ListValue> listValues { get; set; }
    }

    public class ListValue
    {
        public object type { get; set; }
        public object next { get; set; }
        public string value { get; set; }
    }

    public class LibraryDynamicFields
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<FieldsData> data { get; set; }
    }


}
