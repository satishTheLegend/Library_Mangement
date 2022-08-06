using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Model.ApiResponse
{
    public class LogModel
    {
        public string LogType { get; set; }
        public string DeviceName { get; set; }
        public string NetworkProvider { get; set; }
        public string AndroidLevel { get; set; }
        public string BuildNumber { get; set; }
        public string DeviceManufacturer { get; set; }
        public string AppName { get; set; }
        public string Source { get; set; } // FilePath
        public string Module { get; set; } 
        public string FileName { get; set; } // FileName
        public string Method { get; set; } // Method
        public string MethodParams { get; set; } // Method Params
        public string Message { get; set; } // Exception Message
        public string SubMessage { get; set; } // Message
        public string ExtraInfo { get; set; } // Inner Exception
        public DateTime Logdate { get; set; }
    }
}
