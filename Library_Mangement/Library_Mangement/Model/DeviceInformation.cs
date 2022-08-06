using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model
{
    public class DeviceInformation
    {
        public string DeviceID { get; set; }
        public string DeviceManufacturer { get; set; }
        public string AppVersion { get; set; }
        public string PackageName { get; set; }
        public string OSVersion { get; set; }
        public string DeviceModel { get; set; }
        public string AppName { get; set; }
        public string Platform { get; set; }
        public string BuildNumber { get; set; }
        public string NetworkOperatorName { get; set; }
        public DeviceInformation()
        {
            DeviceID = "";
            DeviceManufacturer = "";
            AppVersion = "";
            PackageName = "";
            OSVersion = "";
            DeviceModel = "";
            AppName = "";
            Platform = "";
            BuildNumber = "";
            NetworkOperatorName = "";
        }
    }
}
