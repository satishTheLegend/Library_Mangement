using Library_Mangement.Helper;
using Library_Mangement.Model.ApiResponse;
using Library_Mangement.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.ViewModels
{
    public class LandingViewModel : ValidatableBase
    {
        #region Properties

        #endregion

        #region Constructor
        public LandingViewModel()
        {

        }
        #endregion

        #region Commands

        #endregion

        #region Event Handlers

        #endregion

        #region Public Methods
        public async Task DownloadMasterData()
        {
            try
            {
                var jsonDataBytes = await App.RestServiceConnection.DownloadJsonData("https://drive.google.com/u/0/uc?id=1LE2UDRgqiLC1Pbhvx5jIvNLUvPcivqdV&export=download");
                if (jsonDataBytes != null)
                {
                    var directoryPath = Common.GetBasePath("MasterData");
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    string booksJsonData = Path.Combine(directoryPath, "Books.Json");
                    await Common.SaveFileFromByteArray(jsonDataBytes, booksJsonData);
                    if (File.Exists(booksJsonData))
                    {
                        using (StreamReader r = new StreamReader(booksJsonData))
                        {
                            string json = r.ReadToEnd();
                            List<BooksJsonData> booksJson = JsonConvert.DeserializeObject<List<BooksJsonData>>(json);
                            if (booksJson?.Count > 0)
                            {

                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {

            }
            await Task.FromResult(Task.CompletedTask);
        }
        #endregion

        #region Private Methods

        #endregion
    }
}
