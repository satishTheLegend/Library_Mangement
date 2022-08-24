using ICSharpCode.SharpZipLib.Zip;
using Library_Mangement.Controls;
using Library_Mangement.Model;
using Library_Mangement.Model.ApiResponse;
using Library_Mangement.Services;
using Library_Mangement.Services.PlatformServices;
using Library_Mangement.ViewModels;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Library_Mangement.Helper
{
    public static class Common
    {
        #region Properties
        private static readonly string _moduleName = nameof(Common);
        #endregion

        #region Constructor

        #endregion

        #region Public Methods

        #region Device Info
        public static DeviceInformation DeviceDetails()
        {
            DeviceInformation result = new DeviceInformation();
            try
            {
                result.AppName = AppInfo.Name;
                result.AppVersion = AppInfo.VersionString;
                result.BuildNumber = AppInfo.BuildString;
                result.PackageName = AppInfo.PackageName;
                result.Platform = $"{DeviceInfo.Platform}";
                result.DeviceManufacturer = DeviceInfo.Manufacturer;
                result.DeviceModel = DeviceInfo.Model;
                result.OSVersion = DeviceInfo.VersionString;
                result.DeviceID = DependencyService.Get<IDeviceInfo>().DeviceID;
                result.NetworkOperatorName = DependencyService.Get<IDeviceInfo>().NetworkOperatorName;
            }
            catch (Exception ex)
            {
            }
            return result;
        }
        #endregion

        #region App Releated Methods
        public static string GetBuildMode
        {
            get
            {
                string result = "";
                try
                {
                    var packageName = AppInfo.PackageName;
                    switch (packageName)
                    {
                        case AppConfig.AppPackage_Development:
                            result = "DEV";
                            break;

                        case AppConfig.AppPackage_Staging:
                            result = "STAGE";
                            break;

                        case AppConfig.AppPackage_Production:
                            result = "PROD";
                            break;

                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }
        #endregion

        #region Object Serialization Method
        public static string JsonConvertSerializeObject(object data)
        {
            string strdetails = "";
            try
            {
                strdetails = JsonConvert.SerializeObject(data);
            }
            catch (Exception ex)
            {
                strdetails = $"JsonSerializeException = >> {ex.Message} <<";
                //Task.Run(async () => await App.LogDatabase.Log.AddDataLogs(AppConfig.Log_Info, _strModuleName, $"JsonConvertSerializeObject Exception :::: {ex.StackTrace}"));
            }
            return strdetails;
        }
        #endregion

        public static async Task<bool> GetCameraPermission()
        {
            bool result = false;
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.Camera>();

                if (status == PermissionStatus.Granted)
                    result = true;

            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        public static bool ValidateInputField(DynamicPropertyDataViewModel fieldItem)
        {
            bool isFailedValidated = false;

            switch (fieldItem.InputType.ToLowerInvariant())
            {
                case "text":
                case "password":
                    if (fieldItem == null || string.IsNullOrEmpty(fieldItem.FieldValue))
                    {
                        isFailedValidated = true;
                    }
                    break;

                case "num":
                    fieldItem.ValidationMsg = $"{fieldItem} is required.";
                    if(!long.TryParse(fieldItem.FieldValue, out _))
                    {
                        isFailedValidated = true;
                        fieldItem.ValidationMsg = $"{fieldItem.FieldName} Field can contain only Numeric Values.";
                    }
                    break;

                case "email":
                    fieldItem.ValidationMsg = $"{fieldItem} is required";
                    if (!Regex.IsMatch(fieldItem.FieldValue, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase))
                    {
                        isFailedValidated = true;
                        fieldItem.ValidationMsg = $"This is not valid email.";
                    }
                    break;
            }
            return isFailedValidated;
        }

        public static string GetBasePath(string type)
        {
            string dir = "";
            try
            {
                string basePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Personal)}";
                //string basePath = $"{DependencyService.Get<IFileHelper>().GetPublicFolderPath()}";
                switch (AppInfo.PackageName)
                {
                    case AppConfig.AppPackage_Development:
                        dir = Path.Combine(basePath, AppConfig.AppName, "D", type);
                        break;

                    case AppConfig.AppPackage_Staging:
                        dir = Path.Combine(basePath, AppConfig.AppName, "S", type);
                        break;

                    case AppConfig.AppPackage_Production:
                        dir = Path.Combine(basePath, AppConfig.AppName, "P", type);
                        break;
                }
            }
            catch (Exception ex)
            {
                ExceptionHandlerService.SendErrorLog(_moduleName, ex);
            }
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            return dir;
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
                ExceptionHandlerService.SendErrorLog(_moduleName, ex);
            }
            return await Task.FromResult(fileSaved);
        }

        public static async Task<bool> GetStoragePermission()
        {
            bool result = false;
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();

                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.StorageWrite>();

                if (status == PermissionStatus.Granted)
                    result = true;
            }
            catch (Exception ex)
            {
                ExceptionHandlerService.SendErrorLog(_moduleName, ex);
                result = false;
            }
            return result;
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

        public static Keyboard GetKeyboardType(string keyboardType)
        {
            Keyboard keyboard = null;
            try
            {
                switch (keyboardType.ToLowerInvariant())
                {
                    case "text":
                        keyboard = Keyboard.Text;
                        break;

                    case "password":
                        keyboard = Keyboard.Default;
                        break;

                    case "num":
                        keyboard = Keyboard.Numeric;
                        break;

                    case "email":
                        keyboard = Keyboard.Email;
                        break;

                    default:
                        keyboard = Keyboard.Default;
                        break;
                }
            }
            catch (Exception ex)
            {

            }
            return keyboard;
        }

        public static async Task<bool> UnzipFileAsync(string zipFilePath, string unzipFolderPath)
        {
            try
            {
                if (!Directory.Exists(unzipFolderPath))
                {
                    Directory.CreateDirectory(unzipFolderPath);
                }
                System.IO.Compression.ZipFile.ExtractToDirectory(zipFilePath, unzipFolderPath);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }

        public static async Task<string> DownloadFileAndGETFilePath(string url, string directoryName, string fileName)
        {
            string filePath = string.Empty;
            if (!string.IsNullOrEmpty(url))
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

        public static async Task<bool> UnzipFileAsync(byte[] zipFileBytes, string zipFileName, string unzipFolderPath)
        {
            try
            {
                var tempPath = System.IO.Path.GetTempPath();
                var tempzipFileNewPath = System.IO.Path.Combine(tempPath, $"{zipFileName}.zip");
                bool isZipSaved = await SaveFileFromByteArray(zipFileBytes, tempzipFileNewPath);
                if (!isZipSaved)
                    return false;
                if (Directory.Exists(unzipFolderPath))
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
            catch (Exception ex)
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
                if (!getFilePath)
                {
                    if (!File.Exists(newFilePath))
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


        public static async Task<bool> GetMicroPhonePermission()
        {
            bool result = false;
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();

                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.Microphone>();

                if (status == PermissionStatus.Granted)
                    result = true;
            }
            catch (Exception ex)
            {
                ExceptionHandlerService.SendErrorLog(_moduleName, ex);
                result = false;
            }
            return result;
        }


        public static async Task<bool> GetPhotosPermission()
        {
            bool result = false;
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.Photos>();

                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.Photos>();

                if (status == PermissionStatus.Granted)
                    result = true;
            }
            catch (Exception ex)
            {
                ExceptionHandlerService.SendErrorLog(_moduleName, ex);
                result = false;
            }
            return result;
        }

        public async static Task<string> ClickImageAndGetPath()
        {

            string filePath = string.Empty;
            try
            {
                bool storageStatus = true;
                bool photostatus = true;
                bool camerastatus = await GetCameraPermission();
                if (Device.RuntimePlatform == Device.Android)
                    storageStatus = await Common.GetStoragePermission();
                if (Device.RuntimePlatform == Device.Android)
                    photostatus = await Common.GetPhotosPermission();

                if (camerastatus && storageStatus && photostatus)
                {
                    MediaFile file = await PickImageAsync();
                    if (file == null)
                        return null;
                    filePath = file.Path;
                }
            }
            catch (Exception ex)
            {
                return filePath;
            }
            return filePath;
        }

        public static async Task<MediaFile> PickImageAsync()
        {
            PickMediaOptions pickMedia = new PickMediaOptions
            {
                MaxWidthHeight = 1024,
                PhotoSize = PhotoSize.Large,
                CompressionQuality = 70
            };
            return await CrossMedia.Current.PickPhotoAsync(pickMedia);
        }

        public static async Task<List<MediaFile>> PickMultipleImagesAsync(int maxWidthHeight, int compressionQuality, bool isAllowPermissions, int imageSelectionCount = 5)
        {
            List<MediaFile> files = null;
            try
            {
                if (!isAllowPermissions)
                    return null;

                await CrossMedia.Current.Initialize();

                PickMediaOptions pickMedia = new PickMediaOptions
                {
                    MaxWidthHeight = maxWidthHeight,
                    PhotoSize = PhotoSize.MaxWidthHeight,
                    CompressionQuality = compressionQuality
                };

                MultiPickerOptions multiPicker = new MultiPickerOptions
                {
                    MaximumImagesCount = imageSelectionCount,
                    AlbumSelectTitle = "Select Album",
                    PhotoSelectTitle = "Select Photo",
                    BackButtonTitle = "Back",
                    DoneButtonTitle = "Done",
                    LoadingTitle = "Loading",
                };
                files = await CrossMedia.Current.PickPhotosAsync(pickMedia, multiPicker);

                if (files?.Count == 0 || files == null)
                    return null;
            }
            catch (Exception ex)
            {
                ExceptionHandlerService.SendErrorLog(_moduleName, ex);
            }
            return files;
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
