using Library_Mangement.Services.PlatformServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Library_Mangement.Helper
{
    public static class Common
    {
        #region Properties

        #endregion

        #region Constructor

        #endregion

        #region Public Methods
        public static string GetBasePath(string type)
        {
            string Imagepath = "";
            try
            {
                string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}";
                string packageName = DependencyService.Get<IDeviceInfo>().PackageName;
                switch (packageName)
                {
                    case AppConfig.AppPackage_Development:
                        Imagepath = Path.Combine(basePath, AppConfig.AppName, "D", type);
                        break;

                    case AppConfig.AppPackage_Staging:
                        Imagepath = Path.Combine(basePath, AppConfig.AppName, "S", type);
                        break;

                    case AppConfig.AppPackage_Production:
                        Imagepath = Path.Combine(basePath, AppConfig.AppName, "P", type);
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return Imagepath;
        }

        public static async Task<bool> SaveFileFromByteArray(byte[] fileBytes, string fullFilePath)
        {
            bool fileSaved = false;
            try
            {
                if (!File.Exists(fullFilePath))
                {
                    File.WriteAllBytes(fullFilePath, fileBytes);
                    fileSaved = true;
                }
                else
                {
                    fileSaved = true;
                }
            }
            catch (Exception ex)
            {

            }
            return await Task.FromResult(fileSaved);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
