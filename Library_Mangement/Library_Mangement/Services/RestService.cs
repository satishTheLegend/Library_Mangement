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
using static Library_Mangement.Services.FormUpload;
using Library_Mangement.Helper;
using Xamarin.Forms;
using Library_Mangement.Services.PlatformServices;
using System.Linq;

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
        public bool isBusy { get; set; }
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

        public static async Task<UserRegistrationApiResp> UpdateUser(UserRegistrationPost requestParam)
        {
            var userToken = Preferences.Get(AppConfig.UserPref_UserToken, "");
            Dictionary<string, object> headerParams = new Dictionary<string, object>();
            headerParams.Add(AppConfig.RestService_Param_UserToken, userToken);
            string requestUrl = $"{ServerBaseUrl}{AppConfig.ApiKeypoints_UpdateUserDetails}";
            var data = Newtonsoft.Json.JsonConvert.SerializeObject(requestParam);
            var response = await CrossEDFSCore.Current.PutRequestAsync<UserRegistrationApiResp>(requestUrl, requestParam, headerParams);
            return response;
        }

        public static async Task<T> MasterDataDownload<T>(string apiEndPoint)
        {
            string requestUrl = $"{ServerBaseUrl}{apiEndPoint}";
            var response = await CrossEDFSCore.Current.GetRequest<T>(requestUrl);
            return response;
        }

        #region Download File From URI

        public static async Task<bool> DownloadFileWithProgressUpdated(string fileUrl, string directory, string newFileName = null)
        {
            if (!string.IsNullOrEmpty(newFileName))
            {
                directory = Path.Combine(directory, newFileName);
            }
            else
            {
                directory = Path.Combine(directory, Path.GetFileName(fileUrl));
            }
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength.HasValue ? response.Content.Headers.ContentLength.Value : -1L;
                    var canReportProgress = totalBytes != -1 && totalBytes != 0;

                    using (var contentStream = await response.Content.ReadAsStreamAsync())
                    {
                        var buffer = new byte[4096];
                        var readBytes = 0L;
                        var readBytesThisIteration = 0L;

                        using (var fileStream = new FileStream(directory, FileMode.Create, FileAccess.Write))
                        {
                            do
                            {
                                readBytesThisIteration = await contentStream.ReadAsync(buffer, 0, buffer.Length);

                                await fileStream.WriteAsync(buffer, 0, (int)readBytesThisIteration);

                                readBytes += readBytesThisIteration;

                                if (canReportProgress)
                                {
                                    var progress = (int)(readBytes * 100 / totalBytes);
                                    OnDownloadProgressChanged(progress);
                                }
                            } while (readBytesThisIteration != 0);
                            fileStream.Flush();
                            fileStream.Close();
                        }
                    }
                }
            }
            return true;
        }

        private static void OnDownloadProgressChanged(int progress)
        {
            ProgressEvent?.Invoke(progress);
        }

        public static async Task<string> DownloadFile(string fileUrl, string directory, string newFileName = null, bool isFileDownload = false, bool isAwaitable = false)
        {
            try
            {
                if (!string.IsNullOrEmpty(newFileName))
                {
                    directory = Path.Combine(directory, newFileName);
                }
                else
                {
                    directory = Path.Combine(directory, Path.GetFileName(fileUrl));
                }
                //Task.Run(async () => await DownloadFileFromURIAndSaveIt(fileUrl, directory));
                if (isFileDownload && !isAwaitable)
                {
                    Task.Run(async () => await DownloadFileFromURIAndSaveIt(fileUrl, directory));
                    //await DownloadFileFromURIAndSaveIt(fileUrl, directory);
                }
                else
                {
                    await DownloadFileFromURIAndSaveIt(fileUrl, directory);
                }
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
        public static async Task<bool> DownloadFileWithProgress(string fileUrl, string directory, string newFileName = null)
        {
            if (!string.IsNullOrEmpty(newFileName))
            {
                directory = Path.Combine(directory, newFileName);
            }
            else
            {
                directory = Path.Combine(directory, Path.GetFileName(fileUrl));
            }
            if(Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(fileUrl, HttpCompletionOption.ResponseHeadersRead);
                    var contentLength = response.Content.Headers.ContentLength;
                    var buffer = new byte[1024];
                    var downloadedBytes = 0;
                    long percentValue = 0;
                    if (contentLength.HasValue)
                    {
                        percentValue = contentLength.Value / 100;
                    }
                    using (var fileStream = new FileStream(directory, FileMode.Create, FileAccess.Write, FileShare.None))
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    {
                        while (true)
                        {
                            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                            if (bytesRead == 0)
                            {
                                break;
                            }

                            downloadedBytes += bytesRead;
                            await fileStream.WriteAsync(buffer, 0, bytesRead);

                            if (contentLength.HasValue)
                            {
                                var progress = (double)downloadedBytes / percentValue;
                                ProgressEvent?.Invoke(progress);
                                // Update progress bar or display progress percentage
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
        public static async Task<bool> DownloadUrlFiles(string fileUrl, string directory, string newFileName = null, bool isPDF = false)
        {
            try
            {
                if (!App.IsBusy)
                {

                    App.IsBusy = true;
                    if (!string.IsNullOrEmpty(newFileName))
                    {
                        directory = Path.Combine(directory, newFileName);
                    }
                    else
                    {
                        directory = Path.Combine(directory, Path.GetFileName(fileUrl));
                    }
                    var client = new HttpClientDownloadWithProgress(fileUrl, directory);
                    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                    {
                        //ProgressEvent?.Invoke($"{progressPercentage}% ({totalBytesDownloaded}/{totalFileSize})");
                        ProgressEvent?.Invoke(progressPercentage);
                    };
                    if(File.Exists(newFileName))
                    {
                        await Task.FromResult(true);
                    }
                    await client.StartDownload();
                    if(File.Exists(directory))
                    {
                        string DownloadPath = DependencyService.Get<IFileHelper>().GetPublicFolderPath();
                        DownloadPath = Path.Combine(DownloadPath, newFileName);
                        File.Copy(directory, DownloadPath);
                    }
                    if (isPDF)
                    {
                        await Common.UnzipFileAsync(directory, new FileInfo(directory).Directory.FullName);
                        File.Delete(directory); 
                    }
                    App.IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false); 
            }
            return await Task.FromResult(true);
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

        public static async Task<HttpWebResponse> FileUpload(string filePath, string blobFolderName, bool isImage = false)
        {
            HttpWebResponse result = null;
            //await FileUpload_Updated(filePath);
            try
            {
                var userToken = !string.IsNullOrEmpty(Preferences.Get(AppConfig.UserPref_UserToken, "")) ? Preferences.Get(AppConfig.UserPref_UserToken, "") : "notavailable";

                string requestUrl = $"{ServerBaseUrl}{AppConfig.ApiKeypoints_FileUpload}";
                FileParameter fileparams = null;
                if (isImage)
                {
                    var originalBytes = File.ReadAllBytes(filePath);
                    //var bytesData = await DataUtility.GetCompressedFileBytes(filePath, 5);
                    fileparams = new FileParameter(originalBytes, Path.GetFileName(filePath));
                }
                else
                {
                    fileparams = new FileParameter(File.ReadAllBytes(filePath), Path.GetFileName(filePath));
                }
                Dictionary<string, object> paramsList = new Dictionary<string, object>
                {
                    { "Files", fileparams },
                };
                Dictionary<string, string> headers = new Dictionary<string, string>();
                headers.Add("FolderName", blobFolderName);
                headers.Add(AppConfig.RestService_Param_UserToken, userToken);
                string userAgent = "";
                if (App.CurrentLoggedInUser != null)
                {
                    userAgent = App.CurrentLoggedInUser.Email;
                }
                result = await FormUpload.MultipartFormPost(requestUrl, userAgent, paramsList, headers);
                if (result.StatusCode == HttpStatusCode.OK && isImage)
                {
                    File.Delete(filePath);
                }
                else if (result.StatusCode == HttpStatusCode.OK)
                {
                    await DataUtility.Instance.SaveFileData("DataConfig", Path.GetFileName(filePath));
                }
            }
            catch (Exception ex)
            {

            }
            return result;
        }

        //public static async Task FileUpload_Updated(string filePath)
        //{
        //    var userToken = Preferences.Get(AppConfig.UserPref_UserToken, "");
        //    string requestUrl = $"{ServerBaseUrl}{AppConfig.ApiKeypoints_FileUpload}";
        //    HttpClient httpClient = new HttpClient();
        //    var fileData = new FileInfo(filePath);
        //    MultipartFormDataContent form = new MultipartFormDataContent();
        //    form.Add(new ByteArrayContent(File.ReadAllBytes(fileData.FullName), 0, (int)fileData.Length), "", fileData.Name);
        //    form.Headers.Add(AppConfig.RestService_Param_UserToken, userToken);

        //    HttpResponseMessage response = await httpClient.PostAsync(requestUrl, form);

        //    response.EnsureSuccessStatusCode();
        //    httpClient.Dispose();
        //    string sd = response.Content.ReadAsStringAsync().Result;
        //}

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
                    client.ProgressChanged += (totalFileSize, totalBytesDownloaded, progressPercentage) =>
                    {
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
