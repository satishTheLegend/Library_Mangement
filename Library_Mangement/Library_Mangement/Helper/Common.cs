﻿using ICSharpCode.SharpZipLib.Zip;
using Library_Mangement.Controls;
using Library_Mangement.Services.PlatformServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
        public static byte[] GetByteFromResource(Assembly assemblyData, string fileName, string fileType)
        {
            byte[] byteData = null;
            try
            {
                string[] resourceIDs = assemblyData.GetManifestResourceNames();
                string fileId = resourceIDs.FirstOrDefault(x => x.Contains(fileName) && x.EndsWith(fileType));
                if (!string.IsNullOrEmpty(fileId))
                {
                    using (Stream stream = assemblyData.GetManifestResourceStream(fileId))
                    {
                        if (stream != null)
                        {
                            long length = stream.Length;
                            byteData = new byte[length];
                            stream.Read(byteData, 0, Convert.ToInt32(length));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return byteData;
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

        public static async Task<bool> UnzipFileAsync(byte[] zipFileBytes, string zipFileName, string unzipFolderPath)
        {
            try
            {
                var tempPath = System.IO.Path.GetTempPath();
                var tempzipFileNewPath = System.IO.Path.Combine(tempPath, $"{zipFileName}.zip");
                bool isZipSaved = await SaveFileFromByteArray(zipFileBytes, tempzipFileNewPath);
                if (!isZipSaved)
                    return false;
                if(Directory.Exists(unzipFolderPath))
                {
                    Directory.Delete(unzipFolderPath, true);
                    Directory.CreateDirectory(unzipFolderPath);
                }
                else
                {
                    Directory.CreateDirectory(unzipFolderPath);
                }
                System.IO.Compression.ZipFile.ExtractToDirectory(tempzipFileNewPath, unzipFolderPath);
            }
            catch(Exception ex)
            {
                return false;
            }
            return true;
        }
        
        #endregion

        #region Private Methods

        #endregion
    }
}
