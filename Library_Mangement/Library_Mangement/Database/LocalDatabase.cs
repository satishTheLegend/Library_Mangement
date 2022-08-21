using Library_Mangement.Helper;
using Library_Mangement.Services.PlatformServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Library_Mangement.Database
{
    public class LocalDatabase
    {
        #region Public Methods        
        public async static Task<bool> SaveLoggedInUserData(object userInfo)
        {
            bool res;
            try
            {
                string DatabaseName = string.Format(AppConfig.DatabaseNameFormat.dbNameFormat).ToLower();
                string date = DateTime.Now.ToString("yyyyMMdd");
                string logDatabaseName = string.Format(AppConfig.DatabaseNameFormat.logDBDateName, date).ToLower();
                string dbPath = GetDatabsePath(DatabaseName);
                string dbLogPath = GetDatabsePath(logDatabaseName, true);
                res = Init(dbPath, dbLogPath);
                if (res)
                {
                    //App.CurrentLoggedInUser = await App.Database.User.SaveLoggedInUserDetails(userInfo, DatabaseName, logDatabaseName, DateTime.Now);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return res;
        }
        #endregion

        #region Private Methods
        public static bool Init(string DatabaseFullpath = "", string LogDatabasePath = "")
        {
            try
            {
                string dbFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                if (!Directory.Exists(dbFolderPath))
                {
                    Directory.CreateDirectory(dbFolderPath);
                }

                if (string.IsNullOrEmpty(DatabaseFullpath))
                {
                    string DatabaseName = string.Format(AppConfig.DatabaseNameFormat.dbNameFormat).ToLower();
                    DatabaseFullpath = GetDatabsePath(DatabaseName);
                }

                if(File.Exists(DatabaseFullpath))
                {
                    string DatabaseName = string.Format(AppConfig.DatabaseNameFormat.dbNameFormat).ToLower();
                    var data = Path.Combine(DependencyService.Get<IFileHelper>().GetPublicFolderPath(), DatabaseName);
                    if(!File.Exists(data))
                    {
                        File.Copy(DatabaseFullpath, data);
                    }

                }

                if (App.Database == null)
                    App.Database = new AppDatabase(DatabaseFullpath);


                if (string.IsNullOrEmpty(LogDatabasePath))
                {
                    string date = DateTime.Now.ToString("yyyy_MM_dd");
                    string logDatabaseName = string.Format(AppConfig.DatabaseNameFormat.logDBDateName, date).ToLower();
                    LogDatabasePath = GetDatabsePath(logDatabaseName, true);
                }

                if (App.LogDatabase == null)
                    App.LogDatabase = new LogDatabase(LogDatabasePath);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public static string GetDatabsePath(string DatabseFileName, bool isLogDBPath = false)
        {
            string dbFolderPath = string.Empty;
            string result = string.Empty;
            try
            {
                if (isLogDBPath)
                {
                    dbFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal)); // "EDFS_DSM_Logs"
                }
                else
                {
                    dbFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
                }
                result = Path.Combine(dbFolderPath, DatabseFileName);
            }
            catch (Exception ex)
            {

            }
            return result;
        }
        #endregion
    }
}
