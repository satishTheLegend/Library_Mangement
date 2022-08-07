using Library_Mangement.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EDFSCore;
using Xamarin.Essentials;
using Library_Mangement.Model.ApiResponse.GETModels;
using Library_Mangement.Model.ApiResponse.PostModels;
using Library_Mangement.Model.ApiResponse;

namespace Library_Mangement.Services
{
    public delegate void ProgressBar(double? value);
    public class RestService
    {
        #region Properties
        public static readonly string _strModuleName = nameof(RestService);
        public static string LogParams = string.Empty;
        public const int RequestTimeOutInSeconds = 15;
        public readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(RequestTimeOutInSeconds) };
        public static event ProgressBar ProgressEvent;
        #endregion

        #region Constructor
        public RestService()
        {

        }
        #endregion

        #region Public Methods
        public static async Task<ApiResponseResult<Login>> LoginAsync(string email, string password)
        {
            GetLogParamString(null, true);
            IDictionary<string, string> userData = new Dictionary<string, string>();
            userData.Add(AppConfig.RestService_Param_Email, email);
            userData.Add(AppConfig.RestService_Param_Password, password);
            //userData.Add(AppConfig.RestService_Param_LogParams, LogParams);
            string requestUrl = $"{ServerBaseUrl}{AppConfig.ApiKeypoints_Login}";
            var response = await CrossEDFSCore.Current.PostRequestAsync<Login>(requestUrl, userData);
            return response;
        }
        public static async Task<UserRegistrationApiResp> RegisterUser(UserRegistrationPost requestParam)
        {
            string requestUrl = $"{ServerBaseUrl}{AppConfig.ApiKeypoints_Register}";
            var response = await CrossEDFSCore.Current.PostRequestAsync<UserRegistrationApiResp>(requestUrl, requestParam);
            return response;
        }
        public static async Task<T> MasterDataDownload<T>(string apiEndPoint)
        {
            string requestUrl = $"{ServerBaseUrl}{apiEndPoint}";
            var response = await CrossEDFSCore.Current.GetRequest<T>(requestUrl);
            return response;
        }

        #region Download File From URI
         public static string DownloadFile(string fileUrl, string directory, string newFileName = null)
        {
            try
            {
                if (newFileName != null)
                {
                    directory = Path.Combine(directory, newFileName);
                }
                else
                {
                    Uri uri = new Uri(fileUrl);
                    if (uri.IsFile)
                    {
                        string filename = System.IO.Path.GetFileName(uri.LocalPath);
                        directory = Path.Combine(directory, filename);
                    }
                }
                Task.Run(async () => await DownloadFileFromURIAndSaveIt(fileUrl, directory));
                //if (File.Exists(directory))
                //{
                //    File.Delete(directory);
                //    File.Create(directory);
                //}
                //FileInfo fileInfo = new FileInfo(directory);
                //DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                //if (!directoryInfo.Name.Exists)
                //{
                //    directoryInfo.Create();
                //}
                //using (var client = new HttpClientDownloadWithProgress(fileUrl, directory))
                //{
                //    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                //    {
                //        //ProgressEvent?.Invoke($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                //        ProgressEvent?.Invoke(progressPercentage);
                //    };
                //    await client.StartDownload();
                //}
            }
            catch (Exception ex)
            {

            }
            return directory;
        }

        public static async Task<bool> DownloadFileFromURIAndSaveIt(string fileUrl, string directory)
        {
            bool isSaved = false;
            try
            {
                RestService restService = new RestService();
                var fileBytes = await restService.DownloadFileAsync(fileUrl);
                if (fileBytes != null)
                {
                    isSaved = await Common.SaveFileFromByteArray(fileBytes, directory);
                }
            }
            catch (Exception ex)
            {
                await Task.FromResult(isSaved);
            }
            return await Task.FromResult(isSaved);
        }

        public async Task<byte[]> DownloadFileAsync(string fileUrl)
        {
            try
            {
                using (var httpResponse = await HttpClient.GetAsync(fileUrl))
                {
                    if (httpResponse.StatusCode == HttpStatusCode.OK)
                    {
                        return await httpResponse.Content.ReadAsByteArrayAsync();
                    }
                    else
                    {
                        //Url is Invalid
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandlerService.SendErrorLog(_strModuleName, ex);
                return null;
            }
        }
        #endregion

        public async Task DownloadJsonData(string fileUrl, string destinationFilePath)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(destinationFilePath);
                DirectoryInfo directoryInfo = fileInfo.Directory;
                if (directoryInfo.Exists && destinationFilePath.Contains("MasterDataVerison"))
                {
                    directoryInfo.Delete(true);
                    directoryInfo.Create();
                }

                using (var client = new HttpClientDownloadWithProgress(fileUrl, destinationFilePath))
                {
                    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) => {
                        //ProgressEvent?.Invoke($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                        ProgressEvent?.Invoke(progressPercentage);
                    };

                    await client.StartDownload();
                }

                //using (var httpResponse = await HttpClient.GetAsync(fileUrl))
                //{
                //    if (httpResponse.StatusCode == HttpStatusCode.OK)
                //    {
                //        return await httpResponse.Content.ReadAsByteArrayAsync();
                //    }
                //    else
                //    {
                //        //Url is Invalid
                //        return null;
                //    }
                //}
            }
            catch (Exception ex)
            {
                return;
            }
        }


        #endregion

        #region Private Methods

        #endregion

        #region Headers

        #endregion

        #region Other

        #endregion
        #region Private Methods
        private static string ServerBaseUrl
        {
            get
            {
                string strUrl = string.Empty;
                switch (AppInfo.PackageName)
                {
                    case AppConfig.AppPackage_Development:
                        strUrl = AppConfig.BaseUrl_Development;
                        break;
                    case AppConfig.AppPackage_Staging:
                        strUrl = AppConfig.BaseUrl_Staging;
                        break;
                    case AppConfig.AppPackage_Production:
                        strUrl = AppConfig.BaseUrl_Production;
                        break;
                }
                return strUrl;
            }
        }

        private static Dictionary<string, object> GetAuthHeader()
        {
            Dictionary<string, object> userHeader = new Dictionary<string, object>();
            var userToken = Preferences.Get(AppConfig.UserPref_UserToken, "");
            userHeader.Add(AppConfig.RestService_Param_UserToken, userToken);
            return userHeader;
        }
        private static void GetLogParamString(string StudentId = null, bool forLogin = false)
        {
            try
            {
                string logParams = string.Empty;
                var deviceInfo = Common.DeviceDetails();
                if (deviceInfo != null)
                {
                    if (string.IsNullOrEmpty(StudentId))
                    {
                        StudentId = App.CurrentLoggedInUser == null ? "" : App.CurrentLoggedInUser.RollNo.ToString();
                    }
                    logParams = $"Xamarin | {deviceInfo.Platform}_{deviceInfo.OSVersion} | {deviceInfo.AppVersion} | {deviceInfo.NetworkOperatorName} | {deviceInfo.DeviceManufacturer}_{deviceInfo.DeviceModel} | StudentId - {StudentId}";
                }
                LogParams = logParams;
            }
            catch (Exception ex)
            {
                ExceptionHandlerService.SendErrorLog(_strModuleName, ex);
            }
        }
        #endregion
    }
}
