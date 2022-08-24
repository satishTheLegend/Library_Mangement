using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse.GETModels
{
    public class Settings
    {
        public int id { get; set; }
        public string key { get; set; }
        public string type { get; set; }
        public string value { get; set; }
    }

    public class SettingsGetModel
    {
        public int statusCode { get; set; }
        public string message { get; set; }
        public List<Settings> data { get; set; }
    }
}
