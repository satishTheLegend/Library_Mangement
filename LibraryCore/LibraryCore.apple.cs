using CoreFoundation;
using Foundation;
#if __MAC__
#else
using MobileCoreServices;
#endif
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using UIKit;
using CoreGraphics;
using System.Drawing;
using System.Net;

namespace LibraryCore
{
    public class LibraryFunctionManager : NSUrlSessionDataDelegate, ILibraryCore
    {
#if __MAC__
        IDictionary<string, string> mimeTypes = new Dictionary<string, string>()
        {
              {"html", "text/html"},
              {"htm", "text/html"},
              {"shtml", "text/html"},
              {"css", "text/css"},
              {"xml", "text/xml"},
              {"gif", "image/gif"},
              {"jpeg", "image/jpeg"},
              {"jpg", "image/jpeg"},
              {"js", "application/javascript"},
              {"atom", "application/atom+xml"},
              {"rss", "application/rss+xml"},
              {"mml", "text/mathml"},
              {"txt", "text/plain"},
              {"jad", "text/vnd.sun.j2me.app-descriptor"},
              {"wml", "text/vnd.wap.wml"},
              {"htc", "text/x-component"},
              {"png", "image/png"},
              {"tif", "image/tiff"},
              {"tiff", "image/tiff"},
              {"wbmp", "image/vnd.wap.wbmp"},
              {"ico", "image/x-icon"},
              {"jng", "image/x-jng"},
              {"bmp", "image/x-ms-bmp"},
              {"svg", "image/svg+xml"},
              {"svgz", "image/svg+xml"},
              {"webp", "image/webp"},
              {"woff", "application/font-woff"},
              {"jar", "application/java-archive"},
              {"war", "application/java-archive"},
              {"ear", "application/java-archive"},
              {"json", "application/json"},
              {"hqx", "application/mac-binhex40"},
              {"doc", "application/msword"},
              {"pdf", "application/pdf"},
              {"ps", "application/postscript"},
              {"eps", "application/postscript"},
              {"ai", "application/postscript"},
              {"rtf", "application/rtf"},
              {"m3u8", "application/vnd.apple.mpegurl"},
              {"xls", "application/vnd.ms-excel"},
              {"eot", "application/vnd.ms-fontobject"},
              {"ppt", "application/vnd.ms-powerpoint"},
              {"wmlc", "application/vnd.wap.wmlc"},
              {"kml", "application/vnd.google-earth.kml+xml"},
              {"kmz", "application/vnd.google-earth.kmz"},
              {"7z", "application/x-7z-compressed"},
              {"cco", "application/x-cocoa"},
              {"jardiff", "application/x-java-archive-diff"},
              {"jnlp", "application/x-java-jnlp-file"},
              {"run", "application/x-makeself"},
              {"pl", "application/x-perl"},
              {"pm", "application/x-perl"},
              {"prc", "application/x-pilot"},
              {"pdb", "application/x-pilot"},
              {"rar", "application/x-rar-compressed"},
              {"rpm", "application/x-redhat-package-manager"},
              {"sea", "application/x-sea"},
              {"swf", "application/x-shockwave-flash"},
              {"sit", "application/x-stuffit"},
              {"tcl", "application/x-tcl"},
              {"tk", "application/x-tcl"},
              {"der", "application/x-x509-ca-cert"},
              {"pem", "application/x-x509-ca-cert"},
              {"crt", "application/x-x509-ca-cert"},
              {"xpi", "application/x-xpinstall"},
              {"xhtml", "application/xhtml+xml"},
              {"xspf", "application/xspf+xml"},
              {"zip", "application/zip"},
              {"bin", "application/octet-stream"},
              {"exe", "application/octet-stream"},
              {"dll", "application/octet-stream"},
              {"deb", "application/octet-stream"},
              {"dmg", "application/octet-stream"},
              {"iso", "application/octet-stream"},
              {"img", "application/octet-stream"},
              {"msi", "application/octet-stream"},
              {"msp", "application/octet-stream"},
              {"msm", "application/octet-stream"},
              {"docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
              {"xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
              {"pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
              {"mid", "audio/midi"},
              {"midi", "audio/midi"},
              {"kar", "audio/midi"},
              {"mp3", "audio/mpeg"},
              {"ogg", "audio/ogg"},
              {"m4a", "audio/x-m4a"},
              {"ra", "audio/x-realaudio"},
              {"3gpp", "video/3gpp"},
              {"3gp", "video/3gpp"},
              {"ts", "video/mp2t"},
              {"mp4", "video/mp4"},
              {"mpeg", "video/mpeg"},
              {"mpg", "video/mpeg"},
              {"mov", "video/quicktime"},
              {"webm", "video/webm"},
              {"flv", "video/x-flv"},
              {"m4v", "video/x-m4v"},
              {"mng", "video/x-mng"},
              {"asx", "video/x-ms-asf"},
              {"asf", "video/x-ms-asf"},
              {"wmv", "video/x-ms-wmv"},
              {"avi", "video/x-msvideo"}
        };
#endif

        private static string _actionlogfilename = string.Empty;
        private static List<string> _theUserActions = new List<string>();
        private static string _actionLoggerDirectory = string.Empty;
        public static DateTime timeStamp = DateTime.Now;



        public const string SessionId = "fileuploader";
        public const string UploadFileSuffix = "-multi-part";
        static readonly Encoding encoding = Encoding.UTF8;
        public static Action UrlSessionCompletion { get; set; }
        TaskCompletionSource<FileUploadResponse> uploadCompletionSource;


        // NSMutableData _data = new NSMutableData();
        IDictionary<nuint, NSMutableData> uploadData = new Dictionary<nuint, NSMutableData>();
        public event EventHandler<FileUploadResponse> FileUploadCompleted = delegate { };
        public event EventHandler<FileUploadResponse> FileUploadError = delegate { };
        public event EventHandler<FileUploadProgress> FileUploadProgress = delegate { };


        //Storage Utility Delegates
        TaskCompletionSource<SaveFileResponse> SaveCompletionSource;
        TaskCompletionSource<DeleteFileResponse> DeleteCompletionSource;
        TaskCompletionSource<GetBytesResponse> GetBytesCompletionSource;
        public event EventHandler<SaveFileResponse> SaveFileCompleted = delegate { };
        public event EventHandler<SaveFileResponse> SaveFileError = delegate { };
        public event EventHandler<DeleteFileResponse> DeleteFileSuccess = delegate { };
        public event EventHandler<DeleteFileResponse> DeleteFileError = delegate { };
        public event EventHandler<GetBytesResponse> GetBytesSuccess = delegate { };
        public event EventHandler<GetBytesResponse> GetBytesFail = delegate { };
        public event EventHandler<CreateDbConnection> DbConnSuccess;
        public event EventHandler<CreateDbConnection> DbConnFail;

        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FilePathItem fileItem, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            return await UploadFileAsync(ApiUrl, new FilePathItem[] { fileItem }, fileItem.File, headers, parameters, boundary);
        }
        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FilePathItem[] fileItems, string tag, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            if (fileItems == null || fileItems.Length == 0)
            {
                var fileUploadResponse = new FileUploadResponse("There are no items to upload", -1, tag, null);
                FileUploadError(this, fileUploadResponse);
                return fileUploadResponse;
            }

            bool error = false;
            string errorMessage = string.Empty;

            var uploadItems = new List<UploadFileItemInfo>();
            foreach (var fileItem in fileItems)
            {
                bool temporal = false;
                string path = fileItem.File;
                var tmpPath = path;
                var fileName = tmpPath.Substring(tmpPath.LastIndexOf("/") + 1);
                if (path.StartsWith("/var/"))
                {
                    var data = NSData.FromUrl(new NSUrl($"file://{path}"));
                    tmpPath = SaveToDisk(data, "tmp", fileName);
                    temporal = true;
                }

                if (string.IsNullOrEmpty(boundary))
                {
                    boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                }

                if (File.Exists(tmpPath))
                {
                    uploadItems.Add(new UploadFileItemInfo(tmpPath, fileName, temporal));
                }
                else
                {
                    error = true;
                    errorMessage = $"File at path: {fileItem.File} doesn't exist";
                    break;
                }

            }

            if (error)
            {
                var fileUploadResponse = new FileUploadResponse(errorMessage, -1, tag, null);
                FileUploadError(this, fileUploadResponse);
                return fileUploadResponse;
            }

            var mPath = await SaveToFileSystemAsync(uploadItems.ToArray(), parameters, boundary);



            return await MakeRequest(mPath, tag, ApiUrl, headers, boundary);
        }

        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FileBytesItem fileItem, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            if (string.IsNullOrEmpty(boundary))
            {
                boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            }


            var mPath = await SaveToFileSystemAsync(new UploadFileItemInfo[] { new UploadFileItemInfo(fileItem.Bytes, fileItem.Name) }, parameters, boundary);

            return await MakeRequest(mPath, fileItem.Name, ApiUrl, headers, boundary);
        }
        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FileBytesItem[] fileItems, string tag, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {

            if (string.IsNullOrEmpty(boundary))
            {
                boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            }

            var uploadItems = new List<UploadFileItemInfo>();
            foreach (var fileItem in fileItems)
            {

                uploadItems.Add(new UploadFileItemInfo(fileItem.Bytes, fileItem.Name));

            }

            var mPath = await SaveToFileSystemAsync(uploadItems.ToArray(), parameters, boundary);

            return await MakeRequest(mPath, tag, ApiUrl, headers, boundary);
        }

        async Task<string> SaveToFileSystemAsync(UploadFileItemInfo[] itemsToUpload, IDictionary<string, string> parameters = null, string boundary = null)
        {
            return await Task.Run(() =>
            {
                // Construct the body
                StringBuilder sb = new StringBuilder("");
                if (parameters != null)
                {
                    foreach (string vkp in parameters.Keys)
                    {
                        if (parameters[vkp] != null)
                        {
                            sb.AppendFormat("--{0}\r\n", boundary);
                            sb.AppendFormat("Content-Disposition: form-data; name=\"{0}\"\r\n\r\n", vkp);
                            sb.AppendFormat("{0}\r\n", parameters[vkp]);
                        }
                    }
                }





                string tmpPath = GetOutputPath("tmp", "tmp", null);
                var multiPartPath = $"{tmpPath}{DateTime.Now.ToString("yyyMMdd_HHmmss")}{UploadFileSuffix}";


                // Delete any previous body data file
                if (File.Exists(multiPartPath))
                    File.Delete(multiPartPath);


                using (var writeStream = new FileStream(multiPartPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                {
                    writeStream.Write(encoding.GetBytes(sb.ToString()), 0, encoding.GetByteCount(sb.ToString()));

                    foreach (var fileInfo in itemsToUpload)
                    {
                        sb.Clear();
                        sb.AppendFormat("--{0}\r\n", boundary);
                        sb.Append($"Content-Disposition: form-data; filename=\"{fileInfo.FileName}\"\r\n");
                        sb.Append($"Content-Type: {GetMimeType(fileInfo.FileName)}\r\n\r\n");

                        writeStream.Write(encoding.GetBytes(sb.ToString()), 0, encoding.GetByteCount(sb.ToString()));
                        if (fileInfo.Data != null)
                        {
                            writeStream.Write(fileInfo.Data, 0, fileInfo.Data.Length);

                        }
                        else if (!string.IsNullOrEmpty(fileInfo.OriginalPath) && File.Exists(fileInfo.OriginalPath))
                        {
                            var data = File.ReadAllBytes(fileInfo.OriginalPath);
                            writeStream.Write(data, 0, data.Length);
                        }

                        writeStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                        //delete temporal files created
                        if (fileInfo.IsTemporal && File.Exists(fileInfo.OriginalPath))
                        {
                            File.Delete(fileInfo.OriginalPath);
                        }


                        fileInfo.Data = null;
                    }
                    var pBoundary = $"\r\n--{boundary}--\r\n";
                    writeStream.Write(encoding.GetBytes(pBoundary), 0, encoding.GetByteCount(pBoundary));
                }


                sb = null;
                return multiPartPath;
            });
        }

        NSUrlSessionConfiguration CreateSessionConfiguration(IDictionary<string, string> headers, string identifier, string boundary)
        {
            var sessionConfiguration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(identifier);

            var headerDictionary = new NSMutableDictionary();
            headerDictionary.Add(new NSString("Accept"), new NSString("application/json"));
            headerDictionary.Add(new NSString("Content-Type"), new NSString(string.Format("multipart/form-data; boundary={0}", boundary)));


            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    if (!string.IsNullOrEmpty(headers[key]))
                    {
                        var headerKey = new NSString(key);
                        if (headerDictionary.ContainsKey(new NSString(key)))
                        {
                            headerDictionary[headerKey] = new NSString(headers[key]);
                        }
                        else
                        {
                            headerDictionary.Add(new NSString(key), new NSString(headers[key]));
                        }

                    }
                }
            }


            sessionConfiguration.HttpAdditionalHeaders = headerDictionary;
            sessionConfiguration.AllowsCellularAccess = true;

            sessionConfiguration.NetworkServiceType = NSUrlRequestNetworkServiceType.Default;
            sessionConfiguration.TimeoutIntervalForRequest = 30;
            //sessionConfiguration.HttpMaximumConnectionsPerHost=1;
            //sessionConfiguration.Discretionary = true;
            return sessionConfiguration;
        }

        async Task<FileUploadResponse> MakeRequest(string uploadPath, string tag, string ApiUrl, IDictionary<string, string> headers, string boundary)
        {
            var request = new NSMutableUrlRequest(NSUrl.FromString(ApiUrl));
            request.HttpMethod = "POST";
            request["Accept"] = "*/*";
            request["Content-Type"] = "multipart/form-data; boundary=" + boundary;
            uploadCompletionSource = new TaskCompletionSource<FileUploadResponse>();

            var sessionConfiguration = CreateSessionConfiguration(headers, $"{SessionId}{uploadPath}", boundary);

            var session = NSUrlSession.FromConfiguration(sessionConfiguration, (INSUrlSessionDelegate)this, NSOperationQueue.MainQueue);

            var uploadTask = session.CreateUploadTask(request, new NSUrl(uploadPath, false));

            uploadTask.TaskDescription = $"{tag}|{uploadPath}";
            uploadTask.Priority = NSUrlSessionTaskPriority.High;
            uploadTask.Resume();


            var retVal = await uploadCompletionSource.Task;

            return retVal;
        }


        public override void DidCompleteWithError(NSUrlSession session, NSUrlSessionTask task, NSError error)
        {
            Console.WriteLine(string.Format("DidCompleteWithError TaskId: {0}{1}", task.TaskIdentifier, (error == null ? "" : " Error: " + error.Description)));
            NSMutableData _data = null;

            if (uploadData.ContainsKey(task.TaskIdentifier))
            {
                _data = uploadData[task.TaskIdentifier];
                uploadData.Remove(task.TaskIdentifier);
            }
            else
            {
                _data = new NSMutableData();
            }

            NSString dataString = NSString.FromData(_data, NSStringEncoding.UTF8);
            var message = dataString == null ? string.Empty : $"{dataString}";
            var responseError = false;
            NSHttpUrlResponse response = null;

            string[] parts = task.TaskDescription.Split('|');

            if (task.Response is NSHttpUrlResponse)
            {
                response = task.Response as NSHttpUrlResponse;
                Console.WriteLine("HTTP Response {0}", response);
                Console.WriteLine("HTTP Status {0}", response.StatusCode);
                responseError = response.StatusCode != 200 && response.StatusCode != 201;
            }

            System.Diagnostics.Debug.WriteLine("COMPLETE");

            //Remove the temporal multipart file
            if (parts != null && parts.Length > 0 && File.Exists(parts[1]))
            {
                File.Delete(parts[1]);
            }

            if (parts == null || parts.Length == 0)
                parts = new string[] { string.Empty, string.Empty };

            IDictionary<string, string> responseHeaders = new Dictionary<string, string>();
            var rHeaders = response.AllHeaderFields;
            if (rHeaders != null)
            {
                foreach (var rHeader in rHeaders)
                {
                    if (!string.IsNullOrEmpty($"{rHeader.Value}"))
                    {
                        responseHeaders.Add($"{rHeader.Key}", $"{rHeader.Value}");
                    }
                }
            }

            if (error == null && !responseError)
            {

                var fileUploadResponse = new FileUploadResponse(message, (int)response?.StatusCode, parts[0], new ReadOnlyDictionary<string, string>(responseHeaders));
                uploadCompletionSource.TrySetResult(fileUploadResponse);
                FileUploadCompleted(this, fileUploadResponse);

            }
            else if (responseError)
            {
                var fileUploadResponse = new FileUploadResponse(message, (int)response?.StatusCode, parts[0], new ReadOnlyDictionary<string, string>(responseHeaders));
                uploadCompletionSource.TrySetResult(fileUploadResponse);
                FileUploadError(this, fileUploadResponse);
            }
            else
            {
                var fileUploadResponse = new FileUploadResponse(error.Description, (int)response?.StatusCode, parts[0], new ReadOnlyDictionary<string, string>(responseHeaders));
                uploadCompletionSource.TrySetResult(fileUploadResponse);
                FileUploadError(this, fileUploadResponse);
            }

            _data = null;
        }

        public override void DidReceiveData(NSUrlSession session, NSUrlSessionDataTask dataTask, NSData data)
        {
            System.Diagnostics.Debug.WriteLine("DidReceiveData...");
            if (uploadData.ContainsKey(dataTask.TaskIdentifier))
            {
                uploadData[dataTask.TaskIdentifier].AppendData(data);
            }
            else
            {
                var uData = new NSMutableData();
                uData.AppendData(data);
                uploadData.Add(dataTask.TaskIdentifier, uData);
            }
            // _data.AppendData(data);
        }

        public override void DidReceiveResponse(NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlResponse response, Action<NSUrlSessionResponseDisposition> completionHandler)
        {
            System.Diagnostics.Debug.WriteLine("DidReceiveResponse:  " + response.ToString());

            completionHandler.Invoke(NSUrlSessionResponseDisposition.Allow);
        }

        public override void DidBecomeDownloadTask(NSUrlSession session, NSUrlSessionDataTask dataTask, NSUrlSessionDownloadTask downloadTask)
        {
            System.Diagnostics.Debug.WriteLine("DidBecomeDownloadTask");
        }


        public override void DidBecomeInvalid(NSUrlSession session, NSError error)
        {
            System.Diagnostics.Debug.WriteLine("DidBecomeInvalid" + (error == null ? "undefined" : error.Description));
        }


        public override void DidFinishEventsForBackgroundSession(NSUrlSession session)
        {
            System.Diagnostics.Debug.WriteLine("DidFinishEventsForBackgroundSession");

            if (UrlSessionCompletion != null)
            {
                var completionHandler = UrlSessionCompletion;

                UrlSessionCompletion = null;

                DispatchQueue.MainQueue.DispatchAsync(() =>
                {
                    completionHandler();
                });
            }


        }

        public override void DidSendBodyData(NSUrlSession session, NSUrlSessionTask task, long bytesSent, long totalBytesSent, long totalBytesExpectedToSend)
        {
            string[] parts = task.TaskDescription.Split('|');

            var tag = string.Empty;

            if (parts != null && parts.Length > 0)
            {
                tag = parts[0];
            }

            var fileUploadProgress = new FileUploadProgress(totalBytesSent, totalBytesExpectedToSend, tag);
            FileUploadProgress(this, fileUploadProgress);

            System.Diagnostics.Debug.WriteLine(string.Format("DidSendBodyData bSent: {0}, totalBSent: {1} totalExpectedToSend: {2}", bytesSent, totalBytesSent, totalBytesExpectedToSend));
        }



        string GetOutputPath(string directoryName, string bundleName, string name)
        {
#if __MAC__
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), directoryName);
#else
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), directoryName);
#endif
            Directory.CreateDirectory(path);

            if (string.IsNullOrWhiteSpace(name))
            {
                string timestamp = DateTime.Now.ToString("yyyMMdd_HHmmss");

                name = $"{bundleName}_{timestamp}.jpg";
            }


            return Path.Combine(path, GetUniquePath(path, name));
        }

        string GetUniquePath(string path, string name)
        {

            string ext = Path.GetExtension(name);
            if (ext == string.Empty)
                ext = ".jpg";

            name = Path.GetFileNameWithoutExtension(name);

            string nname = name + ext;
            int i = 1;
            while (File.Exists(Path.Combine(path, nname)))
                nname = name + "_" + (i++) + ext;


            return Path.Combine(path, nname);


        }

        string SaveToDisk(NSData data, string bundleName, string fileName = null, string directoryName = null)
        {


            NSError err = null;
            string path = GetOutputPath(directoryName ?? bundleName, bundleName, fileName);

            if (!File.Exists(path))
            {

                if (data.Save(path, true, out err))
                {
                    System.Diagnostics.Debug.WriteLine("saved as " + path);
                    Console.WriteLine("saved as " + path);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("NOT saved as " + path +
                        " because" + err.LocalizedDescription);
                }

            }

            return path;
        }

        public string GetMimeType(string fileName)
        {
#if __MAC__
            try
            {
                var extensionWithDot = Path.GetExtension(fileName);
                if (!string.IsNullOrWhiteSpace(extensionWithDot))
                {
                    var extension = extensionWithDot.Substring(1);
                    if (!string.IsNullOrWhiteSpace(extension)&&mimeTypes.ContainsKey(extension))
                    {
                       return mimeTypes[extension];
                    }
                }
            }catch(Exception ex)
            {

            }
#else
            try
            {
                var extensionWithDot = Path.GetExtension(fileName);
                if (!string.IsNullOrWhiteSpace(extensionWithDot))
                {
                    var extension = extensionWithDot.Substring(1);
                    if (!string.IsNullOrWhiteSpace(extension))
                    {
                        var extensionClassRef = new NSString(UTType.TagClassFilenameExtension);
                        var mimeTypeClassRef = new NSString(UTType.TagClassMIMEType);

                        var uti = NativeTools.UTTypeCreatePreferredIdentifierForTag(extensionClassRef.Handle, new NSString(extension).Handle, IntPtr.Zero);
                        var mimeType = NativeTools.UTTypeCopyPreferredTagWithClass(uti, mimeTypeClassRef.Handle);
                        using (var mimeTypeCString = new CoreFoundation.CFString(mimeType))
                        {
                            return mimeTypeCString;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
#endif


            return "*/*";
        }

        public Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality)
        {
            UIImage originalImage = ImageFromByteArray(imageData);

            float oldWidth = (float)originalImage.Size.Width;
            float oldHeight = (float)originalImage.Size.Height;
            float scaleFactor = 0f;

            if (oldWidth > oldHeight)
            {
                scaleFactor = width / oldWidth;
            }
            else
            {
                scaleFactor = height / oldHeight;
            }

            float newHeight = oldHeight * scaleFactor;
            float newWidth = oldWidth * scaleFactor;

            //create a 24bit RGB image
            using (CGBitmapContext context = new CGBitmapContext(IntPtr.Zero,
                (int)newWidth, (int)newHeight, 8,
                (int)(4 * newWidth), CGColorSpace.CreateDeviceRGB(),
                CGImageAlphaInfo.PremultipliedFirst))
            {

                RectangleF imageRect = new RectangleF(0, 0, newWidth, newHeight);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                UIKit.UIImage resizedImage = UIKit.UIImage.FromImage(context.ToImage());

                // save the image as a jpeg
                return Task.FromResult(resizedImage.AsJPEG((float)quality).ToArray());
            }
        }

        public static UIKit.UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIKit.UIImage image;
            try
            {
                image = new UIKit.UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception e)
            {
                Console.WriteLine("Image load failed: " + e.Message);
                return null;
            }
            return image;
        }

        public Task<CreateDbConnection> DbConnection(string DbName, string DbPath)
        {
            throw new NotImplementedException();
        }

        public Task<SaveFileResponse> SaveFile(byte[] ImageData, string FilePath)
        {
            throw new NotImplementedException();
        }

        public Task<SaveFileResponse> SaveFile(string SourceFilePath, string DestFilePath)
        {
            throw new NotImplementedException();
        }

        public async Task<GetBytesResponse> GetByte(string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                GetBytesCompletionSource = new TaskCompletionSource<GetBytesResponse>();

                string FileDirectory = System.IO.Path.GetDirectoryName(FilePath);
                string FileName = System.IO.Path.GetFileName(FilePath);
                string NewFileName = FileName.ToLowerInvariant();
                string NewFilePath = System.IO.Path.Combine(FileDirectory, NewFileName);

                if (System.IO.File.Exists(NewFilePath))
                {
                    byte[] Array = System.IO.File.ReadAllBytes(NewFilePath);

                    var ResponseSaved = new GetBytesResponse("File Byte Array is Succesfull", Array);
                    GetBytesSuccess(this, ResponseSaved);
                    GetBytesCompletionSource.TrySetResult(ResponseSaved);
                }
                else
                {
                    var ResponseSaved = new GetBytesResponse("File Path Not Exist", null);
                    GetBytesFail(this, ResponseSaved);
                    GetBytesCompletionSource.TrySetResult(ResponseSaved);
                }
            }
            else
            {
                var ResponseSaved = new GetBytesResponse("Please Check your File Path", null);
                GetBytesFail(this, ResponseSaved);
                GetBytesCompletionSource.TrySetResult(ResponseSaved);
            }
            return await GetBytesCompletionSource.Task;
        }


        public Task<DeleteFileResponse> DeleteFile(string FilePath)
        {
            throw new NotImplementedException();
        }

        public Task<SaveFileResponse> CreateThumbnail(string FilePath, string DestFilePath, float ImgWidth = 0, float ImgHight = 0, int ImgQuality = 0)
        {
            throw new NotImplementedException();
        }

        public Task<SaveFileResponse> CreateThumbnail(byte[] ImageData, string DestFilePath, float ImgWidth = 0, float ImgHight = 0, int ImgQuality = 0)
        {
            throw new NotImplementedException();
        }

        /*public Task SaveFile(string FolderBasePath, string FolderName, string FileFullPath, string FileName, Stream data)
        {
            var directoryPath = Path.Combine(FolderBasePath, FolderName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = Path.Combine(directoryPath, FileName);

            byte[] bArray = new byte[data.Length];
            using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (data)
                {
                    data.Read(bArray, 0, (int)data.Length);
                }
                int length = bArray.Length;
                fs.Write(bArray, 0, length);
            }
            return uploadCompletionSource.Task;
        }*/

        #region "Api Actions"
        //PostRequestAsync function
        public async Task<T> PostRequestAsync<T>(string ApiUrl, IDictionary<string, string> Parameters, IDictionary<string, string> headers = null)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            T response = default(T);

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);

                string jsonstring = JsonConvert.SerializeObject(Parameters);
                HttpContent content = new StringContent(jsonstring, Encoding.UTF8, "application/json");

                string responseText = "";

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }

                var resp = await client.PostAsync(ApiUrl, content);
                responseText = await resp.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseText))
                {
                    response = Deserialize<T>(responseText);
                }
            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response = Deserialize<T>(resString);
                };
            }
            catch (OperationCanceledException operationex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException jsonException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }

            return await Task.FromResult(response);
        }

        public async Task<T> PostRequestAsync<T>(string ApiUrl, object Parameters, IDictionary<string, string> Headers = null)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            T response = default(T);

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);

                string jsonstring = JsonConvert.SerializeObject(Parameters);
                HttpContent content = new StringContent(jsonstring, Encoding.UTF8, "application/json");

                string responseText = "";

                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }

                var resp = await client.PostAsync(ApiUrl, content);
                responseText = await resp.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseText))
                {
                    response = Deserialize<T>(responseText);
                }
            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response = Deserialize<T>(resString);
                };
            }
            catch (OperationCanceledException operationex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException jsonException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }

            return await Task.FromResult(response);
        }
        public async Task<T> PostRequestAsync<T>(string ApiUrl, object Parameters, IDictionary<string, object> Headers = null)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            T response = default(T);

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);

                string jsonstring = JsonConvert.SerializeObject(Parameters);
                HttpContent content = new StringContent(jsonstring, Encoding.UTF8, "application/json");

                string responseText = "";

                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }

                var resp = await client.PostAsync(ApiUrl, content);
                responseText = await resp.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseText))
                {
                    response = Deserialize<T>(responseText);
                }
            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response = Deserialize<T>(resString);
                };
            }
            catch (OperationCanceledException operationex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException jsonException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }

            return await Task.FromResult(response);

        }
        public async Task<T> PostApiRequestAsync<T>(string ApiUrl, object Parameters, IDictionary<string, object> Headers = null)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            T response = default(T);
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);

                string jsonstring = JsonConvert.SerializeObject(Parameters);
                HttpContent content = new StringContent(jsonstring, Encoding.UTF8, "application/json");

                string responseText = "";

                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }

                var resp = await client.PostAsync(ApiUrl, content);
                responseText = await resp.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseText))
                {
                    response = Deserialize<T>(responseText);
                }
            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response = Deserialize<T>(resString);
                };
            }
            catch (OperationCanceledException operationex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException jsonException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            return response;
        }

        public async Task<T> GetRequest<T>(string ApiUrl, IDictionary<string, string> Parameters, IDictionary<string, string> Headers = null)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            T response = default(T);

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);
                string responseText = "";
                string jsonstring = JsonConvert.SerializeObject(Parameters);

                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }


                var resp = await client.GetAsync(ApiUrl);
                responseText = await resp.Content.ReadAsStringAsync();
                response = Deserialize<T>(responseText);

            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response = Deserialize<T>(resString);
                };
            }
            catch (OperationCanceledException operationex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException jsonException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }

            return await Task.FromResult(response);
        }

        public async Task<T> GetRequest<T>(string ApiUrl, Dictionary<string, object> requestParams, Dictionary<string, object> HeaderParams = null)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            T response = default(T);

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);
                string paramsFormatted = "";
                string responseText = "";
                if (requestParams != null && requestParams.Count > 0)
                {
                    paramsFormatted = string.Join("&", requestParams.Select(x => x.Key + "=" + (x.Value)));
                    ApiUrl = ApiUrl + "?" + paramsFormatted;
                }
                if (HeaderParams != null)
                {
                    foreach (var item in HeaderParams)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }
                var resp = await client.GetAsync(ApiUrl);
                responseText = await resp.Content.ReadAsStringAsync();
                response = Deserialize<T>(responseText);

            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response = Deserialize<T>(resString);
                };
            }
            catch (OperationCanceledException operationex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException jsonException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }

            return await Task.FromResult(response);
        }
        public async Task<T> PutRequestAsync<T>(string ApiUrl, object Parameters, IDictionary<string, object> Headers = null)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            T response = default(T);
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);

                string jsonstring = JsonConvert.SerializeObject(Parameters);
                HttpContent content = new StringContent(jsonstring, Encoding.UTF8, "application/json");

                string responseText = "";

                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }

                var resp = await client.PutAsync(ApiUrl, content);
                responseText = await resp.Content.ReadAsStringAsync();


                if (!string.IsNullOrEmpty(responseText))
                {
                    response = Deserialize<T>(responseText);
                }
            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response = Deserialize<T>(resString);
                };
            }
            catch (OperationCanceledException operationex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                errorResponse.Errors = new List<ErrorViewModel>() { new ErrorViewModel() { Key = "OperationCanceledException", Message = operationex.StackTrace } };
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException jsonException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                errorResponse.Errors = new List<ErrorViewModel>() { new ErrorViewModel() { Key = "jsonException", Message = jsonException.StackTrace } };
                response = Deserialize<T>(resString);
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                errorResponse.Errors = new List<ErrorViewModel>() { new ErrorViewModel() { Key = "Exception", Message = ex.StackTrace } };
                response = Deserialize<T>(resString);
            }
            return response;
        }
        //To  Deserialize JObject
        private T Deserialize<T>(string result)
        {

            var toReturn = JsonConvert.DeserializeObject<T>(result);
            return toReturn;
        }

        #endregion

        #region "CreateLogFile"

        // Create Log File using parameters
        public async Task<SaveFileResponse> CreateLogFile(string FileName, string FilePath, string ExtentionForFile)
        {
            SaveCompletionSource = new TaskCompletionSource<SaveFileResponse>();
            try
            {
                if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(ExtentionForFile))
                {
                    var LogFilePath = Path.Combine(FilePath, $"{FileName}.{ExtentionForFile}");
                    using (System.IO.File.Create(LogFilePath))

                        if (System.IO.File.Exists(LogFilePath))
                        {
                            var ResponseSaved = new SaveFileResponse("File saved Succesfully", LogFilePath, null);
                            SaveFileCompleted(this, ResponseSaved);
                            SaveCompletionSource.TrySetResult(ResponseSaved);
                        }
                        else
                        {
                            var ResponseSaved = new SaveFileResponse("File Not Saved", LogFilePath, null);
                            SaveFileError(this, ResponseSaved);
                            SaveCompletionSource.TrySetResult(ResponseSaved);
                        }
                }
                else
                {
                    if (!string.IsNullOrEmpty(FileName))
                    {
                        var ResponseSaved = new SaveFileResponse("FileName is null", null, null);
                        SaveFileError(this, ResponseSaved);
                        SaveCompletionSource.TrySetResult(ResponseSaved);
                    }
                    else if (!string.IsNullOrEmpty(FilePath))
                    {
                        var ResponseSaved = new SaveFileResponse("FilePath is null", null, null);
                        SaveFileError(this, ResponseSaved);
                        SaveCompletionSource.TrySetResult(ResponseSaved);
                    }
                    else
                    {
                        var ResponseSaved = new SaveFileResponse("Extention for file is null", null, null);
                        SaveFileError(this, ResponseSaved);
                        SaveCompletionSource.TrySetResult(ResponseSaved);
                    }
                }
            }
            catch (Exception ex)
            {
                var ResponseSaved = new SaveFileResponse(ex.ToString(), FileName, null);
                SaveFileError(this, ResponseSaved);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                SaveCompletionSource.TrySetResult(ResponseSaved);
            }

            return await SaveCompletionSource.Task;
        }

        #endregion

        #region "LOG Methods"

        // Create  Directory
        public void Logger(string ProvidedactionLoggerDirectory, string Foldername, string FileName)
        {
            string ActionLoggerDirectory = null;

            if (!string.IsNullOrEmpty(ProvidedactionLoggerDirectory))
            {
                ActionLoggerDirectory = Path.Combine(ProvidedactionLoggerDirectory, Foldername); ;
                if (!Directory.Exists(ActionLoggerDirectory))
                {
                    Directory.CreateDirectory(ActionLoggerDirectory);
                }
            }

            _actionlogfilename = FileName;
            _actionLoggerDirectory = ActionLoggerDirectory;
        }

        //Sending User Actions to WriteLogActionToFile Function
        public void LogAction(string FrmName, string CtrlName, string EventName, string Value, List<string> UserLogInfo)
        {
            if (Value?.Length > 10) Value = Value.Substring(0, 10);


            string UserData = null;
            for (int i = 0; i < UserLogInfo.Count; i++)
            {
                UserData = string.Join("\t", UserLogInfo[i].ToString());
            }
            _theUserActions.Add(string.Format("{0}\t{1}\t{2}\t{3}\t{4}{5}", DateTime.UtcNow.ToString(), FrmName, CtrlName, EventName, Value, UserData));
            WriteLogActionsToFile();
        }

        //Getting Log file name and returning path
        public string GetLogFileName()
        {
            string[] ExistingFileList = System.IO.Directory.GetFiles(_actionLoggerDirectory, _actionlogfilename + DateTime.UtcNow.ToString("yyyyMMdd") + "*.log");

            string filePath = _actionLoggerDirectory + _actionlogfilename + DateTime.UtcNow.ToString("yyyyMMdd") + "-0.log";
            if (ExistingFileList.Count() > 0)
            {
                filePath = _actionLoggerDirectory + _actionlogfilename + DateTime.Now.ToString("yyyyMMdd") + "-" + (ExistingFileList.Count() - 1).ToString() + ".log";
            }

            return filePath;
        }

        //Getting Todays Log Files
        public string[] GetTodaysLogFileNames()
        {
            string[] ExistingFileList = System.IO.Directory.GetFiles(_actionLoggerDirectory, _actionlogfilename + DateTime.Now.ToString("yyyyMMdd") + "*.log");
            return ExistingFileList;
        }

        //Wrting Actions in the Log File
        public void WriteLogActionsToFile()
        {
            string LogFilePath = GetLogFileName();
            if (File.Exists(LogFilePath))
            {
                try
                {
                    File.AppendAllLines(LogFilePath, _theUserActions);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            else
            {
                try
                {
                    File.WriteAllLines(LogFilePath, _theUserActions);
                }
                catch (Exception ex)
                {
                    ex.Message.ToString();
                }
            }
            _theUserActions = new List<string>();
        }

        //Method to send Log to Api
        public async Task<FileUploadResponse> LogsUploadApiAsync(string ApiUrl, FilePathItem fileItem, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            return await UploadFileAsync(ApiUrl, new FilePathItem[] { fileItem }, fileItem.File, headers, parameters, boundary);
        }

        //To Delete Logs
        public async Task<string[]> DeleteLog(string LogFilePath, int Days)
        {
            string[] files = Directory.GetFiles(LogFilePath);

            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddDays(-Days))
                        fi.Delete();
                }
            }
            return await Task.FromResult(files);
        }


        public Task<bool> LogServiceAsync(string FilePath, string FileName, Dictionary<string, string> UserData)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LogServiceAsync(string FilePath, string FileName, List<string> UserData)
        {
            throw new NotImplementedException();
        }

        public Task<FileUploadResponse> UploadFileAsync(string ApiUrl, object FileBody, IDictionary<string, string> headers = null, IDictionary<string, string> QueryParam = null, string boundary = null)
        {
            throw new NotImplementedException();
        }

        public async Task<T> ImageUploadPostRequestAsync<T>(string ApiUrl, string Jsonstring, IDictionary<string, object> Headers = null)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            T response = default(T);

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);

                HttpContent content = new StringContent(Jsonstring, Encoding.UTF8, "application/json");

                string responseText = "";

                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }

                var resp = await client.PostAsync(ApiUrl, content);
                responseText = await resp.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseText))
                {
                    response = Deserialize<T>(responseText);
                }
            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response = Deserialize<T>(resString);
                };
            }
            catch (OperationCanceledException operationex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException jsonException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }

            return await Task.FromResult(response);
        }

        async Task<ApiResponseResult<T>> ILibraryCore.PostRequestAsync<T>(string ApiUrl, IDictionary<string, string> Parameters, IDictionary<string, string> Headers)
        {
            ApiResponseResult<object> errorResponse = new ApiResponseResult<object>();
            //T response = default(T);
            int statusCode = 0;
            ApiResponseResult<T> response = new ApiResponseResult<T>();

            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);

                string jsonstring = JsonConvert.SerializeObject(Parameters);
                HttpContent content = new StringContent(jsonstring, Encoding.UTF8, "application/json");

                string responseText = "";

                if (Headers != null)
                {
                    foreach (var item in Headers)
                    {
                        var headerValue = item.Value == null ? string.Empty : item.Value.ToString();
                        client.DefaultRequestHeaders.Add(item.Key, headerValue);
                    }
                }

                var resp = await client.PostAsync(ApiUrl, content);
                statusCode = (int)resp.StatusCode;
                responseText = await resp.Content.ReadAsStringAsync();

                if (!string.IsNullOrEmpty(responseText))
                {
                    response.Result = Deserialize<T>(responseText);
                    response.StatusCode = statusCode;
                }
            }
            catch (WebException webex)
            {
                errorResponse.Status = "-2";
                using (WebResponse webResp = webex.Response)
                {
                    errorResponse.Message = webex.Message;
                    var resString = JsonConvert.SerializeObject(errorResponse);
                    response.Result = Deserialize<T>(resString);
                    response.StatusCode = statusCode;
                };
            }
            catch (OperationCanceledException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response.Result = Deserialize<T>(resString);
                response.StatusCode = statusCode;
            }
            catch (JsonReaderException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "We are not able to connect to Server. Please try after sometime.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response.Result = Deserialize<T>(resString);
                response.StatusCode = statusCode;
            }
            catch (Exception ex)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = ex.Message;
                var resString = JsonConvert.SerializeObject(errorResponse);
                response.Result = Deserialize<T>(resString);
                response.StatusCode = statusCode;
            }
            return await Task.FromResult(response);
        }


        #endregion

    }



    internal class NativeTools
    {
        [DllImport(ObjCRuntime.Constants.MobileCoreServicesLibrary, EntryPoint = "UTTypeCopyPreferredTagWithClass")]
        public extern static IntPtr UTTypeCopyPreferredTagWithClass(IntPtr uti, IntPtr tagClass);

        [DllImport(ObjCRuntime.Constants.MobileCoreServicesLibrary, EntryPoint = "UTTypeCreatePreferredIdentifierForTag")]
        public extern static IntPtr UTTypeCreatePreferredIdentifierForTag(IntPtr tagClass, IntPtr tag, IntPtr uti);
    }
}