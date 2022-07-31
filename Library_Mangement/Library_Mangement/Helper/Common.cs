using ICSharpCode.SharpZipLib.Zip;
using Library_Mangement.Controls;
using Library_Mangement.Model.ApiResponse;
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
            if(!string.IsNullOrEmpty(Imagepath) && !Directory.Exists(Imagepath))
            {
                Directory.CreateDirectory(Imagepath);
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

        public static bool UnzipFileAsync(string zipFilePath, string unzipFolderPath)
        {
            try
            {
                if(Directory.Exists(unzipFolderPath))
                {
                    Directory.Delete(unzipFolderPath, true);
                }
                Directory.CreateDirectory(unzipFolderPath);
                System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, unzipFolderPath);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static async Task<string> DownloadFileAndGETFilePath(string url, string directoryName, string fileName)
        {
            string filePath = string.Empty;
            if(!string.IsNullOrEmpty(url))
            {
                filePath = Path.Combine(GetBasePath(directoryName), fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                await App.RestServiceConnection.DownloadJsonData(url, filePath);
                //if(fileBytes != null)       byte[] fileBytes = 
                //{
                //    filePath = Path.Combine(GetBasePath(directoryName), fileName);
                //    if(File.Exists(filePath))
                //    {
                //        File.Delete(filePath);
                //    }
                //    bool isFileSaved = await SaveFileFromByteArray(fileBytes, filePath);
                //    filePath = !isFileSaved ? "" : filePath;
                //}
            }
            return await Task.FromResult(filePath);
        }

        public static string GetFileNameFromURL(string url)
        {
            string filename = string.Empty;
            Uri uri = new Uri(url);
            if (uri.IsFile)
            {
                filename = System.IO.Path.GetFileName(uri.LocalPath);
            }
            return filename;
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

        public static async Task<string> SaveImageThumbnails(string thumbnailsPath, string thumbnailUrl, bool getFilePath = false)
        {
            string path = string.Empty;
            if (!string.IsNullOrEmpty(thumbnailUrl))
            {
                Uri uri = new Uri(thumbnailUrl);
                string filename = Path.GetFileName(uri.AbsolutePath);
                string newFilePath = Path.Combine(thumbnailsPath, filename);
                if(!getFilePath)
                {
                    if(!File.Exists(newFilePath))
                    {
                        await App.RestServiceConnection.DownloadJsonData(thumbnailUrl, newFilePath);
                        //if (imagedata == null) return path; var imagedata = 
                        //bool isSaved = await SaveFileFromByteArray(imagedata, newFilePath);
                        //if (isSaved)
                        //{
                        //    path = newFilePath;
                        //}
                    }
                    else
                    {
                        path = newFilePath;
                    }
                }
                else
                {
                    path = newFilePath;
                }
            }
            return path;
        }

        public static string GetPNGFilePath(string directoryName, string pdfFileName)
        {
            return Path.Combine(GetBasePath(directoryName), pdfFileName);
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
