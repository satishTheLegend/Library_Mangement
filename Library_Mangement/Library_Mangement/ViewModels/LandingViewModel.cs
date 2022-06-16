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
                await LoaderMessage("Master Data Download Begin.", 1700);
                LoaderVisible = true;
                LandingPageEnable = false;
                LandingPageOpacity = 0.5;
                var directoryPath = Common.GetBasePath("MasterData");
                bool isImagesSaved = await ExtractImagesFromZip(directoryPath);
                await LoaderMessage("Images Saved Successfully.", 1500);
                if (!isImagesSaved)
                    isImagesSaved = true;
                await LoaderMessage("Downloading Json Data.", 1500);
                var jsonDataBytes = await App.RestServiceConnection.DownloadJsonData("https://drive.google.com/u/0/uc?id=1LE2UDRgqiLC1Pbhvx5jIvNLUvPcivqdV&export=download");
                if (jsonDataBytes != null)
                {
                    await LoaderMessage("Json Data Downloaded Successfully.", 1500);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    string booksJsonData = Path.Combine(directoryPath, "Books.Json");
                    await Common.SaveFileFromByteArray(jsonDataBytes, booksJsonData);
                    if (File.Exists(booksJsonData))
                    {
                        await LoaderMessage("Json File Stored On Your Local Device.", 900);
                        using (StreamReader r = new StreamReader(booksJsonData))
                        {
                            string json = r.ReadToEnd();
                            List<BooksJsonData> booksJson = JsonConvert.DeserializeObject<List<BooksJsonData>>(json);
                            await LoaderMessage("Reading Data From Json.", 800);
                            if (booksJson?.Count > 0)
                            {
                                await LoaderMessage("Json Parsed Successfully.", 800);
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
                                    await LoaderMessage($"Added Book Records {i} Out Of {booksJson?.Count}.", 150);
                                    i++;
                                }
                                await LoaderMessage($"Books Added Successfully.", 100);
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
                await LoaderMessage($"Setting Up All Files.", 1400);
                string finishingText = "Please Wait Finishing Up";
                for (int i = 0; i < 12; i++)
                {
                    switch (i)
                    {
                        case 8:
                            finishingText = "Finializing";
                            break;

                        case 10:
                            finishingText = "Process Completed";
                            break;
                    }

                    for (int j = 1; j <= 4; j++)
                    {
                        string progress = string.Concat(Enumerable.Repeat(".", j));
                        await LoaderMessage($"{finishingText}{progress}", 400);
                    }

                }
                LoaderVisible = false;
                LandingPageEnable = true;
                LandingPageOpacity = 1.0;
                await Task.FromResult(Task.CompletedTask);
            }
        }

        private async Task LoaderMessage(string loaderText, int timeDeley)
        {
            LoaderText = $"{loaderText}";
            await Task.Delay(timeDeley);
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
