using Library_Mangement.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Services
{
    public delegate void ProgressBar(double? value);
    public class RestService
    {
        #region Properties
        public const int RequestTimeOutInSeconds = 15;
        public readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(RequestTimeOutInSeconds) };
        public event ProgressBar ProgressEvent;
        #endregion

        #region Constructor
        public RestService()
        {

        }
        #endregion

        #region Public Methods
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

        #region Handlers

        #endregion
    }
}
