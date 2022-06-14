using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Services
{
    public class RestService
    {
        #region Properties
        public const int RequestTimeOutInSeconds = 15;
        public readonly HttpClient HttpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(RequestTimeOutInSeconds) };
        #endregion

        #region Constructor
        public RestService()
        {

        }
        #endregion

        #region Public Methods
        public async Task<byte[]> DownloadJsonData(string fileUrl)
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
                return null;
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
