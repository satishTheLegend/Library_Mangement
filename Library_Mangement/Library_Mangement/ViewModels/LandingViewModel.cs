using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using Library_Mangement.Model;
using Library_Mangement.Model.ApiResponse;
using Library_Mangement.Model.ApiResponse.GETModels;
using Library_Mangement.Services;
using Library_Mangement.Validation;
using Library_Mangement.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Library_Mangement.ViewModels
{
    public class LandingViewModel : ValidatableBase
    {
        #region Properties
        private Assembly _assembly = typeof(MainPage).GetTypeInfo().Assembly;
        private string FilePath = string.Empty;

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

        private double? _loaderPercent = 0;
        public double? LoaderPercent
        {
            get => _loaderPercent;
            set
            {
                _loaderPercent = value;
                OnPropertyChanged(nameof(LoaderPercent));
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
            App.RestServiceConnection.ProgressEvent += RestServiceConnection_ProgressEvent;
        }
        #endregion

        #region Commands
        public ICommand SignUpCommand => new Command(async () => await SignUP());
        #endregion

        #region Event Handlers
        private async Task SignUP()
        {
            await App.Current.MainPage.Navigation.PushAsync(new UserRegistration());
        }
        #endregion

        #region Public Methods
        public async Task DownloadMasterData_Updated()
        {
            try
            {
                await SyncMasterData();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                LoaderVisible = false;
                LandingPageEnable = true;
                LandingPageOpacity = 1.0;
                await Task.FromResult(Task.CompletedTask);
            }
        }

        private async Task SyncMasterData()
        {
            bool isMasterDataNotAvailable = false;
            var masterData = await RestService.MasterDataDownload<OCLMMasterVeriosnControl>(AppConfig.ApiKeypoints_OCLM_MasterVersionControl);
            foreach (var masterItem in masterData.data)
            {
                isMasterDataNotAvailable = await App.Database.MasterDataVerison.IsMasterDataVersionMissing(masterItem);
            }

            if(isMasterDataNotAvailable)
            {
                BooksJsonMasterData booksMaster = await RestService.MasterDataDownload<BooksJsonMasterData>(AppConfig.ApiKeypoints_BooksMaster);
                OCLMDynamicFields fieldsMaster = await RestService.MasterDataDownload<OCLMDynamicFields>(AppConfig.ApiKeypoints_OCLM_DynamicFields);
                LibraryCodesMaster codesMaster = await RestService.MasterDataDownload<LibraryCodesMaster>(AppConfig.ApiKeypoints_OCLM_Codes);
            }
        }

        public async Task DownloadMasterData()
        {
            try
            {
                bool isCheckUpdate = true;
                var isRecordAdded = await App.Database.Settings.FindByKeyAsync("CheckVersion");
                if (isRecordAdded != null && !string.IsNullOrEmpty(isRecordAdded.Value))
                {
                    DateTime lastVersionCheck = Convert.ToDateTime(isRecordAdded.Value);
                    isCheckUpdate = DateTime.Now > lastVersionCheck.AddHours(6);
                }
                if (isCheckUpdate)
                {
                    await LoadMasterData();
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                LoaderVisible = false;
                LandingPageEnable = true;
                LandingPageOpacity = 1.0;
                await Task.FromResult(Task.CompletedTask);
            }
        }
        #endregion

        #region Private Methods
        private async Task LoadMasterData()
        {
            try
            {
                await LoaderMessage("Master LoginData Download Begin.", 1700);
                LoaderVisible = true;
                LandingPageEnable = false;
                LandingPageOpacity = 0.5;
                await LoaderMessage("Downloading Master LoginData.", 1500);
                List<MasterVerisonModel> verisonDataList = null;
                string masterDataFilePath = await Common.DownloadFileAndGETFilePath("https://drive.google.com/u/0/uc?id=1d_nqcTA8SKhmQRS-0-ZCkrGQFhr5lWxx&export=download", "MasterData", "MasterDataVerison.Json");
                if (File.Exists(masterDataFilePath))
                {
                    using (StreamReader r = new StreamReader(masterDataFilePath))
                    {
                        string masterJson = r.ReadToEnd();
                        await LoaderMessage("Checking For Master LoginData Verison Change.", 800);
                        verisonDataList = JsonConvert.DeserializeObject<List<MasterVerisonModel>>(masterJson);
                        foreach (var versionItem in verisonDataList)
                        {
                            var data = await App.Database.MasterDataVerison.FindItemByKey(versionItem.KeyName);
                            if (data != null)
                            {
                                bool result = data.Verison != versionItem.Verison ? true : false;
                                if (result)
                                {
                                    await SaveVersionRecordsToDB(versionItem);
                                }
                            }
                            else
                            {
                                await SaveVersionRecordsToDB(versionItem);
                            }
                        }
                    }
                    tblSettings settings = new tblSettings()
                    {
                        Key = "CheckVersion",
                        Value = DateTime.Now.ToString()
                    };
                    await App.Database.Settings.InsertAsync(settings);

                    await FinializeMessage();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void RestServiceConnection_ProgressEvent(double? value)
        {
            LoaderPercent = value;
            //await LoaderMessage($"Download Progress {value}", 150);
            //LoaderText = $"Download Progress {value}";
        }

        private async Task FinializeMessage()
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
        }

        private async Task FinalUpdateValues()
        {
            var allBooks = await App.Database.Book.GetDataAsync();
            if (allBooks?.Count <= 0)
                return;
            string[] thumbnailsPath = null;
            var directoryPath = Common.GetBasePath("BookThumbnails");
            List<BooksThumbnailsModel> booksThumbnailsList = new List<BooksThumbnailsModel>();
            if (Directory.Exists(directoryPath))
            {
                //DirectoryInfo dir = new DirectoryInfo(directoryPath);
                thumbnailsPath = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);
            }
            foreach (var item in thumbnailsPath)
            {
                BooksThumbnailsModel booksThumbnails = new BooksThumbnailsModel();
                booksThumbnails.FilePath = item;
                string prevfileName = Path.GetFileName(item);
                var pattern = new Regex("[:!@#$%^&*()}{|\":?><\\-._[\\]\\-;'/.,~]");
                prevfileName = pattern.Replace(prevfileName, " ");
                if (prevfileName.ToLowerInvariant().Contains("png"))
                {
                    prevfileName = prevfileName.ToLowerInvariant().Replace("png", string.Empty);
                }
                if (prevfileName.ToLowerInvariant().Contains("jpg"))
                {
                    prevfileName = prevfileName.ToLowerInvariant().Replace("jpg", string.Empty);
                }
                if (prevfileName.ToLowerInvariant().Contains("pdf"))
                {
                    prevfileName = prevfileName.ToLowerInvariant().Replace("pdf", string.Empty);
                }
                booksThumbnails.FileName = prevfileName;
                booksThumbnailsList.Add(booksThumbnails);
            }
            int count = 0;
            foreach (var bookItem in allBooks)
            {
                var pattern = new Regex("[:!@#$%^&*()}{|\":?><\\-._[\\]\\-;'/.,~]");
                string newfileName = pattern.Replace(bookItem.PdfName, " ");
                if (newfileName.ToLowerInvariant().Contains("png"))
                {
                    newfileName = newfileName.ToLowerInvariant().Replace("png", string.Empty);
                }
                if (newfileName.ToLowerInvariant().Contains("jpg"))
                {
                    newfileName = newfileName.ToLowerInvariant().Replace("jpg", string.Empty);
                }
                if (newfileName.ToLowerInvariant().Contains("pdf"))
                {
                    newfileName = newfileName.ToLowerInvariant().Replace("pdf", string.Empty);
                }

                var filePath = string.Empty;
                if (booksThumbnailsList?.Count > 0)
                {
                    filePath = booksThumbnailsList.FirstOrDefault(x => Regex.Replace(x.FileName, @"\s+", "").ToLowerInvariant().Contains(Regex.Replace(newfileName, @"\s+", ""))).FilePath;
                }
                await LoaderMessage($"{count} book added to database", 0);
                LoaderPercent = SetProgressBarValue(count, allBooks.Count);
                bookItem.PngFilePath = filePath;
                await App.Database.Book.UpdateAsync(bookItem);
                count++;
            }
        }

        private async Task SaveVersionRecordsToDB(MasterVerisonModel versionItem)
        {
            try
            {
                bool isSaved = false;
                if (!string.IsNullOrEmpty(versionItem.FileExtention))
                {
                    if (versionItem.FileExtention.ToLowerInvariant() == "zip")
                        isSaved = await SaveZipDataAsync(versionItem);
                    else
                        isSaved = await DownloadAllMasterDataFiles(versionItem);
                }

                if (string.IsNullOrEmpty(versionItem.FileName))
                {
                    FilePath = string.Empty;
                }
                tblVerisonMaster verisonMaster = new tblVerisonMaster()
                {
                    KeyName = versionItem.KeyName,
                    Link = versionItem.Link,
                    Verison = versionItem.Verison,
                    Active = versionItem.Active,
                    IsRecordSaveToDB = versionItem.IsRecordSaveToDB,
                    FilePath = FilePath,
                    DirectoryName = versionItem.DirectoryName,
                    FileExtention = versionItem.FileExtention,
                    FileName = versionItem.FileName,
                    Value = versionItem.Value,
                    IsRecordSaved = isSaved,
                };
                await App.Database.MasterDataVerison.InsertAsync(verisonMaster);

                if (versionItem.KeyName == "Catagories")
                {
                    await SaveCatagories();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task SaveCatagories()
        {
            var catgoryJson = await App.Database.MasterDataVerison.FindItemByKey("Catagories");
            if (catgoryJson != null && !string.IsNullOrEmpty(catgoryJson.FilePath) && File.Exists(catgoryJson.FilePath))
            {
                using (StreamReader r = new StreamReader(catgoryJson.FilePath))
                {
                    string json = r.ReadToEnd();
                    List<CatagoryModel> CatagoriesJson = JsonConvert.DeserializeObject<List<CatagoryModel>>(json);
                    int i = 1;
                    foreach (var catagoryItem in CatagoriesJson)
                    {
                        tblCodesMaster tblCodes = new tblCodesMaster();
                        tblCodes.CodeId = catagoryItem.CatagoryId;
                        tblCodes.CodeText = catagoryItem.CatagoryName;
                        tblCodes.CodeValue = await Common.DownloadFileAndGETFilePath(catagoryItem.CatagorySvgUrl, "Master", $"{catagoryItem.CatagoryId}.svg");
                        await LoaderMessage($"Adding Book Catagories {i} Out Of {CatagoriesJson?.Count}.", 0);
                        LoaderPercent = SetProgressBarValue(i, CatagoriesJson.Count);
                        await App.Database.CodesMaster.InsertAsync(tblCodes);
                        i++;
                    }
                }
            }
        }

        private async Task<bool> SaveZipDataAsync(MasterVerisonModel masterDataItem)
        {
            bool isAllFilesSaved = false;
            try
            {
                await LoaderMessage($"Books Image Download Started", 0);
                string masterZIPFilePath = await Common.DownloadFileAndGETFilePath(masterDataItem.Link, masterDataItem.DirectoryName, masterDataItem.FileName);
                if (File.Exists(masterZIPFilePath))
                {
                    FilePath = masterZIPFilePath;
                    await LoaderMessage($"Images Downloaded Successfully", 500);
                    await LoaderMessage($"Extracting Images", 500);
                    string thumbnailsFilePath = Common.GetBasePath("BookThumbnails");
                    isAllFilesSaved = Common.UnzipFileAsync(masterZIPFilePath, thumbnailsFilePath);
                    await LoaderMessage($"Images Saved", 500);
                    await FinalUpdateValues();
                }
            }
            catch (Exception ex)
            {

            }
            return isAllFilesSaved;
        }

        private async Task<bool> DownloadAllMasterDataFiles(MasterVerisonModel versionItem)
        {
            bool isDataDownloaded = false;
            try
            {
                string masterFilePath = await Common.DownloadFileAndGETFilePath(versionItem.Link, versionItem.DirectoryName, versionItem.FileName);
                if (File.Exists(masterFilePath))
                {
                    FilePath = masterFilePath;
                    if (!versionItem.IsRecordSaveToDB)
                        return true;
                    await LoaderMessage($"{versionItem.FileName} LoginData Downloaded Successfully.", 500);
                    await LoaderMessage($"{versionItem.FileName} File Stored On Your Local Device.", 900);
                    using (StreamReader r = new StreamReader(masterFilePath))
                    {
                        string json = r.ReadToEnd();
                        if(versionItem.KeyName == "LibraryDynamicFields")
                        {
                            LibraryDynamicFields dynamicFields = JsonConvert.DeserializeObject<LibraryDynamicFields>(json);
                            await LoaderMessage($"Reading LoginData From {versionItem.FileName}.", 800);
                            if (dynamicFields?.data != null && versionItem.IsRecordSaveToDB)
                            {
                                await LoaderMessage($"{versionItem.FileName} Parsed Successfully.", 800);
                                int i = 0;
                                foreach (var dynamicDataItem in dynamicFields?.data)
                                {
                                    tblLibraryDynamicFields libraryDynamicFieldsRecord = new tblLibraryDynamicFields()
                                    {
                                        ControlType = dynamicDataItem.ControlType,
                                        FieldId = dynamicDataItem.FieldId,
                                        FieldName = dynamicDataItem.FieldName,
                                        PageName = dynamicDataItem.PageName,
                                        Required = dynamicDataItem.Required,
                                        Sequence = dynamicDataItem.Sequence,
                                        GroupName = dynamicDataItem.GroupName,
                                        KeyboardType = dynamicDataItem.KeyboardType,
                                        Validation = dynamicDataItem.Validation,
                                        ValidationMsg = dynamicDataItem.ValidationMsg,
                                        ListValues = JsonConvert.SerializeObject(dynamicDataItem.listValues)
                                    };
                                    await App.Database.LibraryDynamicFields.InsertAsync(libraryDynamicFieldsRecord);
                                    await LoaderMessage($"Added Fields Records {i} Out Of {dynamicFields?.data?.Count}.", 0);
                                    LoaderPercent = SetProgressBarValue(i, dynamicFields.data.Count);
                                    i++;
                                }
                                isDataDownloaded = true;
                                await LoaderMessage($"Fields Records Added Successfully.", 100);
                            }

                        }
                        else
                        {
                            List<BooksJsonData> booksJson = JsonConvert.DeserializeObject<List<BooksJsonData>>(json);
                            await LoaderMessage($"Reading LoginData From {versionItem.FileName}.", 800);
                            if (booksJson?.Count > 0 && versionItem.IsRecordSaveToDB)
                            {
                                await LoaderMessage($"{versionItem.FileName} Parsed Successfully.", 800);
                                //await LoaderMessage("Started To Download Book Thumbnails.", 800);
                                //int thumb = 0;
                                //foreach (var bookImageItem in booksJson)
                                //{
                                //    await Task.Run(async () => await Common.SaveImageThumbnails(thumbnailsPath, bookImageItem.thumbnailUrl));
                                //    await LoaderMessage($"Downloading Image {thumb} Out Of {booksJson.Count}.", 0);
                                //    thumb++;
                                //}
                                //await LoaderMessage("Book Thumbnails Download Completed.", 800);
                                int i = 0;
                                foreach (var bookItem in booksJson)
                                {
                                    tblBook bookRecord = new tblBook()
                                    {
                                        Title = bookItem.Title,
                                        ISBN = bookItem.ISBN,
                                        PageCount = bookItem.PageCount,
                                        //PngFilePath = await Common.SaveImageThumbnails(thumbnailsPath, bookItem.thumbnailUrl, true),
                                        PublishedDate = bookItem.PublishDate,
                                        PngLink = bookItem.PNGFileLink,
                                        PngName = bookItem.ImageName,
                                        PngFilePath = Common.GetPNGFilePath("BookThumbnails", bookItem.PdfFileName),
                                        PdfLink = bookItem.PDFFileLink,
                                        PdfName = bookItem.PdfFileName,
                                        Status = bookItem.Status,
                                        PdfFilePath = "",
                                        Authors = bookItem.AuthorName,
                                        Categories = bookItem.Catagories,
                                    };
                                    bookRecord.IsCoverAvailable = !string.IsNullOrEmpty(bookRecord.PngFilePath) ? true : false;
                                    await App.Database.Book.InsertAsync(bookRecord);
                                    await LoaderMessage($"Added Book Records {i} Out Of {booksJson?.Count}.", 0);
                                    LoaderPercent = SetProgressBarValue(i, booksJson.Count);
                                    i++;
                                }
                                isDataDownloaded = true;
                                await LoaderMessage($"Books Added Successfully.", 100);
                            }
                        }
                        
                    }

                }
            }
            catch (Exception ex)
            {

            }
            return isDataDownloaded;
        }

        private double? SetProgressBarValue(int i, int count)
        {
            double runtimePercent = 0.0;
            double percentValue = count / 100;
            if (percentValue > 0)
            {
                runtimePercent = i / percentValue;
            }
            return runtimePercent;
        }

        private async Task LoaderMessage(string loaderText, int timeDeley)
        {
            LoaderText = $"{loaderText}";
            if (timeDeley > 0 && AppConfig.isAwaitTimeNeeds)
            {
                await Task.Delay(timeDeley);
            }
        }
        #endregion
    }
}
