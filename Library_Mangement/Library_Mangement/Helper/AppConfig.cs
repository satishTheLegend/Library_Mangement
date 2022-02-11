using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Helper
{
    public class AppConfig
    {
        public const string ApplicationName_DEV = "AppResources DEV";
        public const string ApplicationName_Stage = "AppResources STAGE";
        public const string ApplicationName_Prod = "AppResources";
        public const string ModuleName = "AppResources";


        //Database Format
        public static class DatabaseNameFormat
        {
            public static string dbNameFormat = "Library_Mangement.DB.sqlite";
            public static string logDBName = "Library_Mangement.DB.Log.sqlite";
            public static string logDBDateName = "Library_Mangement.ConsoleLogs.Dev.{0}.sqlite";
            public static string centralDBName = "Central.sqlite";
        }

        //App Theme
        public const string AppTheme_Theme = "Default";

        //Session Check
        public const int SessionCheck_Minutes = 1380; //1380 - 23 hours
        public const int SessionTimeOut_Minutes = 1440;//1440 - 24 hours

    }
}
