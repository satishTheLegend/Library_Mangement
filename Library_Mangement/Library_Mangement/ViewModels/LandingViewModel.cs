using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model.ApiResponse;
using Library_Mangement.Validation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.ViewModels
{
    public class LandingViewModel : ValidatableBase
    {
        #region Properties
        private Assembly _assembly = typeof(MainPage).GetTypeInfo().Assembly;

        private bool _loaderVisible = false;
        public bool LoaderVisible
        {
            get => _loaderVisible;
            set
            {
                _loaderVisible = value;
                OnPropertyChanged(nameof(LoaderVisible));
            }
        }
        private bool _landingPageEnable = true;
        public bool LandingPageEnable
        {
            get => _landingPageEnable;
            set
            {
                _landingPageEnable = value;
                OnPropertyChanged(nameof(LandingPageEnable));
            }
        }

        private double _landingPageOpacity = 1.0;
        public double LandingPageOpacity
        {
            get => _landingPageOpacity;
            set
            {
                _landingPageOpacity = value;
                OnPropertyChanged(nameof(LandingPageOpacity));
            }
        }

        private string _loaderText;
        public string LoaderText
        {
            get => _loaderText;
            set
            {
                _loaderText = value;
                OnPropertyChanged(nameof(LoaderText));
            }
        }


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
                LoaderText = "Master Data Download Begin.";
                await Task.Delay(1700);
                LoaderVisible = true;
                LandingPageEnable = false;
                LandingPageOpacity = 0.5;
                var directoryPath = Common.GetBasePath("MasterData");
                bool isImagesSaved = await ExtractImagesFromZip(directoryPath);
                LoaderText = "Images Saved Successfully.";
                await Task.Delay(1500);
                if (!isImagesSaved)
                    isImagesSaved = true;
                LoaderText = "Downloading Json Data.";
                await Task.Delay(1500);
                var jsonDataBytes = await App.RestServiceConnection.DownloadJsonData("https://drive.google.com/u/0/uc?id=1LE2UDRgqiLC1Pbhvx5jIvNLUvPcivqdV&export=download");
                if (jsonDataBytes != null)
                {
                    LoaderText = "Json Data Downloaded Successfully.";
                    await Task.Delay(1500);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    string booksJsonData = Path.Combine(directoryPath, "Books.Json");
                    await Common.SaveFileFromByteArray(jsonDataBytes, booksJsonData);
                    if (File.Exists(booksJsonData))
                    {
                        LoaderText = "Json File Stored On Your Local Device.";
                        await Task.Delay(900);
                        using (StreamReader r = new StreamReader(booksJsonData))
                        {
                            string json = r.ReadToEnd();
                            List<BooksJsonData> booksJson = JsonConvert.DeserializeObject<List<BooksJsonData>>(json);
                            LoaderText = "Reading Data From Json.";
                            await Task.Delay(800);
                            if (booksJson?.Count > 0)
                            {
                                LoaderText = "Json Parsed Successfully.";
                                await Task.Delay(800);
                                int i = 0;
                                foreach (var bookItem in booksJson)
                                {
                                    bool isFileExist = false;
                                    if (File.Exists($"{directoryPath}/{bookItem.imageLink}"))
                                        isFileExist = true;
                                    tblBook bookRecord = new tblBook()
                                    {
                                        Author = bookItem.author,
                                        Country = bookItem.country,
                                        ImageLink = $"{directoryPath}/{bookItem.imageLink}",
                                        IsCoverAvailable = isFileExist,
                                        Language = bookItem.language,
                                        Link = bookItem.link,
                                        Pages = bookItem.pages,
                                        Title = bookItem.title,
                                        Year = bookItem.year,
                                    };
                                    await App.Database.Book.InsertAsync(bookRecord);
                                    LoaderText = $"Added Book Records {i} Out Of {booksJson?.Count}.";
                                    await Task.Delay(500);
                                    i++;
                                }
                                LoaderText = $"Books Added Successfully.";
                                await Task.Delay(1000);
                            }
                        }

                    }

                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                LoaderText = $"Setting Up All Files.";
                await Task.Delay(1400);
                string finishingText = "Finishing Up";
                for (int i = 0; i < 15; i++)
                {
                    switch (i)
                    {
                        case 5:
                            finishingText = "Finializing";
                            break;

                        case 10:
                            finishingText = "Completed";
                            break;
                    }

                    for (int j = 1; j <= 4; j++)
                    {
                        string progress = string.Concat(Enumerable.Repeat(".", j));
                        LoaderText = $"{finishingText}{progress}";
                        await Task.Delay(500);
                    }

                }
                LoaderVisible = false;
                LandingPageEnable = true;
                LandingPageOpacity = 1.0;
                await Task.FromResult(Task.CompletedTask);
            }
        }

        private async Task<bool> ExtractImagesFromZip(string directoryPath)
        {
            bool result = false;
            try
            {
                string fileName = "images";
                var getImagesZipStream = Common.GetByteFromResource(_assembly, fileName, ".zip");
                result = await Common.UnzipFileAsync(getImagesZipStream, fileName, directoryPath);
            }
            catch (Exception ex)
            {
                return result;
            }
            return result;
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
