using System;
using System.Collections.Generic;
using System.Text;

namespace Library_Mangement.Helper
{
    public class AppConfig
    {
        public const bool isAwaitTimeNeeds = false;

        //Application Log Levels
        public const string LogType_Error = "Error";
        public const string LogType_Debug = "Debug";
        public const string LogType_Info = "Info";

        //Base URL
        public const string BaseUrl_Development = "https://oclmwebapis.azurewebsites.net/api/";  //Base URL.
        public const string BaseUrl_Staging = "https://oclmwebapis.azurewebsites.net/api/";  //Base URL.
        public const string BaseUrl_Production = "https://oclmwebapis.azurewebsites.net/api/";  //Base URL.

        //App Packages 
        public const string AppPackage_Development = "com.companyname.library_mangement.dev";
        public const string AppPackage_Staging = "com.companyname.library_mangement.stage";
        public const string AppPackage_Production = "com.companyname.library_mangement";

        //App Name
        public const string AppName = "library_mangement";

        // App UserPref Constants
        public const string UserPref_LoginId = "LoginId";
        public const string UserPref_RememberMe = "RememberMe";
        public const string UserPref_UserToken = "UserToken";

        // RestService UserPref Constants
        public const string RestService_Param_UserToken = "UserToken";

        // RestService Constants
        public const string RestService_Param_LogParams = "logParams";
        public const string RestService_Param_Email = "email";
        public const string RestService_Param_Password = "password";



        //Database Format
        public static class DatabaseNameFormat
        {
            public const string dbNameFormat = "Library_Mangement.DB.sqlite";
            public const string logDBName = "Library_Mangement.DB.Log.sqlite";
            public const string logDBDateName = "Library_Mangement.ConsoleLogs.Dev.{0}.sqlite";
            public const string centralDBName = "Central.sqlite";
        }

        // Api EndPoint Keys
        public const string ApiKeypoints_Login = "LibraryLogin/Login";
        public const string ApiKeypoints_Register = "LibraryLogin/Register";
        public const string ApiKeypoints_UpdateUserDetails = "LibraryLogin/UpdateUserDetails";
        public const string ApiKeypoints_BooksMaster = "BooksJsonMaster/GetBooks";
        public const string ApiKeypoints_FileUpload = "FileUploadService/UploadFiles";
        public const string ApiKeypoints_OCLM_MasterVersionControl = "LibraryMasterVersion/GetVersion";
        public const string ApiKeypoints_OCLM_DynamicFields = "OCLMDynamicFields/GetFields";
        public const string ApiKeypoints_OCLM_GetCodes = "CodesMaster/GetCodes";
        public const string ApiKeypoints_OCLM_AddCodes = "CodesMaster/AddCodes";

        //App Theme
        public const string AppTheme_Theme = "Default";

        //Session Check
        public const int SessionCheck_Minutes = 1380; //1380 - 23 hours
        public const int SessionTimeOut_Minutes = 1440;//1440 - 24 hours

        // App userDataResp Folder Names
        public const string DirName_Profile_Pic = "UserProfile";
        public const string DirName_Books_Thumbnails = "BookThumbnails";

        public class ValidateMessageType
        {
            public const string Error = "#0#";
            public const string Warning = "#1#";
        }


        //Blob Storage Names
        public const string profile = "profileimage";
        public const string databackup = "databackup";
    }
}
