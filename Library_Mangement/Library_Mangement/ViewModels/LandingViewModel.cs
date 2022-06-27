using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model.ApiResponse;
using Library_Mangement.Validation;
using Library_Mangement.Views;
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
                await LoaderMessage("Downloading Json Data.", 1500);
                bool isMasterDataNotAvailable = false;
                List<MasterVerisonModel> verisonDataList = null;
                var MasterDataVerisonBytes = await App.RestServiceConnection.DownloadJsonData("https://drive.google.com/u/0/uc?id=1vr34-Yx2giOYQCdMUABvB2gN4L4kt_84&export=download");
                if (MasterDataVerisonBytes != null)
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                    string masterDataVerisonJsonPath = Path.Combine(directoryPath, "MasterDataVerison.Json");
                    await Common.SaveFileFromByteArray(MasterDataVerisonBytes, masterDataVerisonJsonPath);
                    using (StreamReader r = new StreamReader(masterDataVerisonJsonPath))
                    {
                        string masterJson = r.ReadToEnd();
                        await LoaderMessage("Checking For Master Data Verison Change.", 800);
                        verisonDataList = JsonConvert.DeserializeObject<List<MasterVerisonModel>>(masterJson);
                        List<bool> masterDataResult = new List<bool>();
                        foreach (var item in verisonDataList)
                        {
                            var data = await App.Database.MasterDataVerison.FindItemByKey(item.KeyName);
                            if(data != null)
                            {
                                bool result = data.Value != item.Value ? false : true;
                                masterDataResult.Add(result);
                            }
                            else
                            {
                                masterDataResult.Add(false);
                            }
                        }
                        bool isMasterDataAvailable = masterDataResult.Any(x => x == false);
                        if(!isMasterDataAvailable)
                        {
                            await LoaderMessage("No New Updates For Master Data.", 400);
                        }
                        else
                        {
                            isMasterDataNotAvailable = true;
                        }
                    }
                }
                if(isMasterDataNotAvailable)
                {
                    var jsonDataBytes = await App.RestServiceConnection.DownloadJsonData("https://drive.google.com/u/0/uc?id=1LE2UDRgqiLC1Pbhvx5jIvNLUvPcivqdV&export=download");
                    if (jsonDataBytes != null)
                    {
                        await LoaderMessage("Json Data Downloaded Successfully.", 500);
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }
                        string booksJsonData = Path.Combine(directoryPath, "Books.Json");
                        string thumbnailsPath = Path.Combine(directoryPath, "Thumbnail");
                        if(!Directory.Exists(thumbnailsPath))
                        {
                            Directory.CreateDirectory(thumbnailsPath);
                        }
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
                                    await LoaderMessage("Started To Download Book Thumbnails.", 800);
                                    int thumb = 0;
                                    foreach (var bookImageItem in booksJson)
                                    {
                                        await Task.Run(async () => await Common.SaveImageThumbnails(thumbnailsPath, bookImageItem.thumbnailUrl));
                                        await LoaderMessage($"Downloading Image {thumb} Out Of {booksJson.Count}.", 0);
                                        thumb++;
                                    }
                                    await LoaderMessage("Book Thumbnails Download Completed.", 800);
                                    int i = 0;
                                    foreach (var bookItem in booksJson)
                                    {
                                        tblBook bookRecord = new tblBook()
                                        {
                                            Title = bookItem.title,
                                            ISBN = bookItem.isbn,
                                            PageCount = bookItem.pageCount,
                                            FilePath = await Common.SaveImageThumbnails(thumbnailsPath, bookItem.thumbnailUrl, true),
                                            PublishedDate = bookItem.publishedDate != null ? bookItem.publishedDate.Date : DateTime.Now,
                                            ThumbnailUrl = bookItem.thumbnailUrl,
                                            ShortDescription = bookItem.shortDescription,
                                            LongDescription = bookItem.longDescription,
                                            Status = bookItem.status,
                                            Authors = bookItem.authors != null && bookItem.authors?.Count > 0 ? string.Join(",", bookItem.authors) : "",
                                            Categories = bookItem.categories != null && bookItem.categories?.Count > 0 ? string.Join(",", bookItem.categories) : "",
                                        };
                                        bookRecord.IsCoverAvailable = !string.IsNullOrEmpty(bookRecord.FilePath) ? true : false;
                                        await App.Database.Book.InsertAsync(bookRecord);
                                        await LoaderMessage($"Added Book Records {i} Out Of {booksJson?.Count}.", 0);
                                        i++;
                                    }
                                    await LoaderMessage($"Books Added Successfully.", 100);
                                }
                            }

                        }

                    }
                    foreach (var item in verisonDataList)
                    {
                        tblVerisonMaster verisonMaster = new tblVerisonMaster()
                        {
                            Key = item.KeyName,
                            Value = item.Value,
                        };
                        await App.Database.MasterDataVerison.InsertAsync(verisonMaster);
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
            if(timeDeley > 0 && AppConfig.isAwaitTimeNeeds)
            {
                await Task.Delay(timeDeley);
            }
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
