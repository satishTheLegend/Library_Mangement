using Android.Graphics;
using Android.Webkit;
using Java.IO;
using Java.Util.Concurrent;
using Newtonsoft.Json;
using OkHttp;
using OkHttp.Okio;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EDFSCore
{
    /// <summary>
    /// Interface for EDFS.DSM
    /// </summary>
    public class EDFSFunctionManager : IEDFSCore, ICountProgressListener
    {
        #region "Constant and Static Members"

        public static TimeUnit UploadTimeoutUnit { get; set; } = TimeUnit.Minutes;
        public static long SocketUploadTimeout { get; set; } = 5;
        public static long ConnectUploadTimeout { get; set; } = 5;

        #endregion

        #region "Private Member Variables"
        private static string _actionlogfilename = string.Empty;
        private static List<string> _theUserActions = new List<string>();
        private static string _actionLoggerDirectory = string.Empty;
        public static DateTime timeStamp = DateTime.Now;

        #endregion

        //Upload File Delegates
        TaskCompletionSource<FileUploadResponse> uploadCompletionSource;
        public event EventHandler<FileUploadResponse> FileUploadCompleted = delegate { };
        public event EventHandler<FileUploadResponse> FileUploadError = delegate { };
        public event EventHandler<FileUploadProgress> FileUploadProgress = delegate { };

        //Database delegates
        TaskCompletionSource<CreateDbConnection> CreateDbSource;
        public event EventHandler<CreateDbConnection> DbConnSuccess = delegate { };
        public event EventHandler<CreateDbConnection> DbConnFail = delegate { };

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


        #region "File Upload Functionality"
        //Method to upload single file Using File Bytes Item.
        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FileBytesItem fileItem, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {
            return await UploadFileAsync(ApiUrl, new FileBytesItem[] { fileItem }, fileItem.Name, headers, parameters, boundary);
        }

        //Method to upload Multiple file Using File Bytes Item.
        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FileBytesItem[] fileItems, string file_name = null, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {

            uploadCompletionSource = new TaskCompletionSource<FileUploadResponse>();

            //Checking File Item or file item length is null or 0.
            if (fileItems == null || fileItems.Length == 0)
            {
                var fileUploadResponse = new FileUploadResponse("There are no items to upload", -1, file_name, null);
                FileUploadError(this, fileUploadResponse);

                uploadCompletionSource.TrySetResult(fileUploadResponse);
            }
            else
            {
                //await tast to upload files.
                await Task.Run(() =>
                {
                    //exception handling.
                    try
                    {
                        var requestBodyBuilder = PrepareRequest(parameters, boundary);

                        foreach (var fileItem in fileItems)
                        {
                            var mediaType = MediaType.Parse(GetMimeType(fileItem.Name));

                            if (mediaType == null)
                                mediaType = MediaType.Parse("*/*");
                            RequestBody fileBody = RequestBody.Create(mediaType, fileItem.Bytes);
                            requestBodyBuilder.AddFormDataPart(fileItem.FileType, fileItem.Name, fileBody);
                        }

                        //getting response from api.
                        var resp = MakeRequest(ApiUrl, file_name, requestBodyBuilder, headers);

                        if (!uploadCompletionSource.Task.IsCompleted)
                        {
                            uploadCompletionSource.TrySetResult(resp);
                        }

                    }
                    catch (Java.Net.UnknownHostException ex)
                    {
                        var fileUploadResponse = new FileUploadResponse("Host not reachable", -1, file_name, null);
                        FileUploadError(this, fileUploadResponse);
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        uploadCompletionSource.TrySetResult(fileUploadResponse);
                    }
                    catch (Java.IO.IOException ex)
                    {
                        var fileUploadResponse = new FileUploadResponse(ex.ToString(), -1, file_name, null);
                        FileUploadError(this, fileUploadResponse);
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        uploadCompletionSource.TrySetResult(fileUploadResponse);
                    }
                    catch (Exception ex)
                    {
                        var fileUploadResponse = new FileUploadResponse(ex.ToString(), -1, file_name, null);
                        FileUploadError(this, fileUploadResponse);
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        uploadCompletionSource.TrySetResult(fileUploadResponse);
                    }


                });
            }

            return await uploadCompletionSource.Task;
        }

        //Getting file extension.
        string GetMimeType(string ApiUrl)
        {
            string type = "*/*";
            try
            {
                string extension = MimeTypeMap.GetFileExtensionFromUrl(ApiUrl);
                if (!string.IsNullOrEmpty(extension))
                {
                    type = MimeTypeMap.Singleton.GetMimeTypeFromExtension(extension.ToLower());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return type;
        }

        //Method to upload single file Using File Path Item.
        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FilePathItem fileItem, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {

            return await UploadFileAsync(ApiUrl, new FilePathItem[] { fileItem }, fileItem.File, headers, parameters, boundary);
        }

        //Method to upload Multiple file Using File Path Item.
        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FilePathItem[] fileItems, string file_name = null, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null)
        {

            uploadCompletionSource = new TaskCompletionSource<FileUploadResponse>();

            //Checking File Item or file item length is null or 0.
            if (fileItems == null || fileItems.Length == 0)
            {
                var fileUploadResponse = new FileUploadResponse("There are no items to upload", -1, file_name, null);
                FileUploadError(this, fileUploadResponse);
                uploadCompletionSource.TrySetResult(fileUploadResponse);
            }
            else
            {
                //await tast to upload files.
                await Task.Run(() =>
                {
                    //exception handling.
                    try
                    {

                        var requestBodyBuilder = PrepareRequest(parameters, boundary);

                        foreach (var fileItem in fileItems)
                        {
                            {
                                //file_name = System.IO.Path.GetFileName(fileItem.Path);
                                file_name = fileItem.File;

                                Java.IO.File f = new Java.IO.File(fileItem.File);
                                string fileAbsolutePath = f.AbsolutePath;

                                RequestBody file_body = RequestBody.Create(MediaType.Parse(GetMimeType(fileItem.File)), f);
                                //var fileName = fileAbsolutePath.Substring(fileAbsolutePath.LastIndexOf("/") + 1);
                                //var fileName = fileAbsolutePath.Substring(fileAbsolutePath.LastIndexOf("/") + 1);
                                requestBodyBuilder.AddFormDataPart(fileItem.FileName, fileItem.FileRelPath, file_body);
                            }
                        }
                        //getting response from api.
                        var resp = MakeRequest(ApiUrl, file_name, requestBodyBuilder, headers);

                        if (!uploadCompletionSource.Task.IsCompleted)
                        {
                            uploadCompletionSource.TrySetResult(resp);
                        }

                    }
                    catch (Java.Net.UnknownHostException ex)
                    {
                        var fileUploadResponse = new FileUploadResponse("Host not reachable", -1, file_name, null);
                        FileUploadError(this, fileUploadResponse);
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        uploadCompletionSource.TrySetResult(fileUploadResponse);
                    }
                    catch (Java.IO.IOException ex)
                    {
                        var fileUploadResponse = new FileUploadResponse(ex.ToString(), -1, file_name, null);
                        FileUploadError(this, fileUploadResponse);
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        uploadCompletionSource.TrySetResult(fileUploadResponse);
                    }
                    catch (Exception ex)
                    {
                        var fileUploadResponse = new FileUploadResponse(ex.ToString(), -1, file_name, null);
                        FileUploadError(this, fileUploadResponse);
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        uploadCompletionSource.TrySetResult(fileUploadResponse);
                    }
                });
            }
            return await uploadCompletionSource.Task;
        }

        //Multipart Builder.
        MultipartBuilder PrepareRequest(IDictionary<string, string> parameters = null, string boundary = null)
        {
            MultipartBuilder requestBodyBuilder = null;

            if (string.IsNullOrEmpty(boundary))
            {
                requestBodyBuilder = new MultipartBuilder()
                        .Type(MultipartBuilder.Form);
            }
            else
            {
                requestBodyBuilder = new MultipartBuilder(boundary)
                        .Type(MultipartBuilder.Form);
            }

            if (parameters != null)
            {
                foreach (string key in parameters.Keys)
                {
                    if (parameters[key] != null)
                    {
                        requestBodyBuilder.AddFormDataPart(key, parameters[key]);
                    }
                }
            }
            return requestBodyBuilder;
        }

        //Getting File Upload response.
        FileUploadResponse MakeRequest(string ApiUrl, string file_name, MultipartBuilder requestBodyBuilder, IDictionary<string, string> headers = null)
        {
            //RequestBody requestBody = requestBodyBuilder.Build();
            CountingRequestBody requestBody = new CountingRequestBody(requestBodyBuilder.Build(), file_name, this);
            var requestBuilder = new Request.Builder();

            if (headers != null)
            {
                foreach (string key in headers.Keys)
                {
                    if (!string.IsNullOrEmpty(headers[key]))
                    {
                        requestBuilder = requestBuilder.AddHeader(key, headers[key]);
                    }
                }
            }

            Request request = requestBuilder
                .Url(ApiUrl)
                .Post(requestBody)
                .Build();

            OkHttpClient client = new OkHttpClient();
            client.SetConnectTimeout(ConnectUploadTimeout, UploadTimeoutUnit); // connect timeout
            client.SetReadTimeout(SocketUploadTimeout, UploadTimeoutUnit);    // socket timeout

            Response response = client.NewCall(request).Execute();
            var responseString = response.Body().String();
            var code = response.Code();

            IDictionary<string, string> responseHeaders = new Dictionary<string, string>();
            var rHeaders = response.Headers();
            if (rHeaders != null)
            {
                var names = rHeaders.Names();
                foreach (string name in names)
                {
                    if (!string.IsNullOrEmpty(rHeaders.Get(name)))
                    {
                        responseHeaders.Add(name, rHeaders.Get(name));
                    }
                }
            }

            //Getting value of file Upload response.
            FileUploadResponse fileUploadResponse = new FileUploadResponse(responseString, code, file_name, new ReadOnlyDictionary<string, string>(responseHeaders));

            if (response.IsSuccessful)
            {


                FileUploadCompleted(this, fileUploadResponse);

            }
            else
            {
                FileUploadError(this, fileUploadResponse);

            }

            return fileUploadResponse;
        }

        //Getting file Upload progress.
        public void OnProgress(string file_name, long bytesWritten, long contentLength)
        {
            var fileUploadProgress = new FileUploadProgress(bytesWritten, contentLength, file_name);
            FileUploadProgress(this, fileUploadProgress);
        }

        //Getting Upload error. 
        public void OnError(string file_name, string error)
        {
            var fileUploadResponse = new FileUploadResponse(error, -1, file_name, null);
            FileUploadError(this, fileUploadResponse);
            System.Diagnostics.Debug.WriteLine(error);


            uploadCompletionSource.TrySetResult(fileUploadResponse);
        }
        #endregion

        public async Task<CreateDbConnection> DbConnection(string DbName, string DbPath)
        {
            if (!string.IsNullOrEmpty(DbName) && !string.IsNullOrEmpty(DbPath))
            {
                CreateDbSource = new TaskCompletionSource<CreateDbConnection>();

                string NewDbName = DbName.ToLowerInvariant();
                string path = System.IO.Path.Combine(DbPath, NewDbName);

                if (System.IO.File.Exists(path))
                {
                    var database = new SQLiteAsyncConnection(path);
                    //_database.CreateTableAsync<abc>().Wait();

                    var ResponseSaved = new CreateDbConnection("File Delete Failed", database.ToString());
                    DbConnSuccess(this, ResponseSaved);
                    CreateDbSource.TrySetResult(ResponseSaved);
                }
                else
                {
                    var ResponseSaved = new CreateDbConnection("No DB File Exist", path);
                    DbConnFail(this, ResponseSaved);
                    CreateDbSource.TrySetResult(ResponseSaved);
                }
            }
            else
            {
                var ResponseSaved = new CreateDbConnection("Please check your Database Name and Database Path", null);
                DbConnFail(this, ResponseSaved);
                CreateDbSource.TrySetResult(ResponseSaved);
            }

            return await CreateDbSource.Task;
        }

        public async Task<SaveFileResponse> SaveFile(byte[] ImageData, string FilePath)
        {
            SaveCompletionSource = new TaskCompletionSource<SaveFileResponse>();

            if (!string.IsNullOrEmpty(FilePath) && ImageData.Length == 0 && ImageData == null)
            {
                string FileDirectory = System.IO.Path.GetDirectoryName(FilePath);
                string FileName = System.IO.Path.GetFileName(FilePath);
                string NewFileName = FileName.ToLowerInvariant();

                if (!Directory.Exists(FileDirectory))
                {
                    DirectoryInfo di = Directory.CreateDirectory(FileDirectory);
                }

                string NewFilePath = System.IO.Path.Combine(FileDirectory, NewFileName);
                using (var fileOutputStream = new FileOutputStream(NewFilePath))
                {
                    await fileOutputStream.WriteAsync(ImageData);
                }

                if (System.IO.File.Exists(NewFilePath))
                {
                    var ResponseSaved = new SaveFileResponse("File saved Succesfully", NewFilePath, null);
                    SaveFileCompleted(this, ResponseSaved);
                    SaveCompletionSource.TrySetResult(ResponseSaved);
                }
                else
                {
                    var ResponseSaved = new SaveFileResponse("File Not Saved", NewFilePath, null);
                    SaveFileError(this, ResponseSaved);
                    SaveCompletionSource.TrySetResult(ResponseSaved);
                }
            }
            else
            {
                var ResponseSaved = new SaveFileResponse("Please check your File Path and Image Data", null, null);
                SaveFileError(this, ResponseSaved);
                SaveCompletionSource.TrySetResult(ResponseSaved);
            }
            return await SaveCompletionSource.Task;
        }

        public async Task<FileUploadResponse> UploadFileAsync(string ApiUrl, object FileBody, IDictionary<string, string> headers = null, IDictionary<string, string> QueryParam = null, string boundary = null)
        {
            uploadCompletionSource = new TaskCompletionSource<FileUploadResponse>();
            try
            {

                var client = new HttpClient();
                client.BaseAddress = new Uri(ApiUrl);

                string jsonstring = JsonConvert.SerializeObject(FileBody);
                HttpContent content = new StringContent(jsonstring, Encoding.UTF8, "multipart/form-data");

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


                if (!uploadCompletionSource.Task.IsCompleted)
                {
                    var fileUploadResponse = new FileUploadResponse(responseText, 1, null, null);
                    FileUploadError(this, fileUploadResponse);
                    uploadCompletionSource.TrySetResult(fileUploadResponse);
                }


            }
            catch (Java.Net.UnknownHostException ex)
            {
                var fileUploadResponse = new FileUploadResponse("Host not reachable", -1, null, null);
                FileUploadError(this, fileUploadResponse);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                uploadCompletionSource.TrySetResult(fileUploadResponse);
            }
            catch (Java.IO.IOException ex)
            {
                var fileUploadResponse = new FileUploadResponse(ex.ToString(), -1, null, null);
                FileUploadError(this, fileUploadResponse);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                uploadCompletionSource.TrySetResult(fileUploadResponse);
            }
            catch (Exception ex)
            {
                var fileUploadResponse = new FileUploadResponse(ex.ToString(), -1, null, null);
                FileUploadError(this, fileUploadResponse);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                uploadCompletionSource.TrySetResult(fileUploadResponse);
            }

            return await uploadCompletionSource.Task;

        }

        public async Task<SaveFileResponse> SaveFile(string SourceFilePath, string DestFilePath)
        {
            if (!string.IsNullOrEmpty(SourceFilePath) && !string.IsNullOrEmpty(DestFilePath))
            {

                if (System.IO.File.Exists(SourceFilePath))
                {
                    byte[] array = System.IO.File.ReadAllBytes(SourceFilePath);
                    return await SaveFile(array, DestFilePath);
                }
                else
                {
                    var ResponseSaved = new SaveFileResponse("Source File Path Does Not Exist", SourceFilePath, null);
                    SaveFileError(this, ResponseSaved);
                    SaveCompletionSource.TrySetResult(ResponseSaved);
                }
            }
            else
            {
                var ResponseSaved = new SaveFileResponse("Please check your Source File Path and Destination File Path", SourceFilePath, null);
                SaveFileError(this, ResponseSaved);
                SaveCompletionSource.TrySetResult(ResponseSaved);
            }
            return await SaveCompletionSource.Task;
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

        public async Task<DeleteFileResponse> DeleteFile(string FilePath)
        {
            if (!string.IsNullOrEmpty(FilePath))
            {
                DeleteCompletionSource = new TaskCompletionSource<DeleteFileResponse>();

                System.IO.File.Delete(FilePath);
                if (System.IO.File.Exists(FilePath))
                {
                    System.IO.File.Delete(FilePath);

                    if (System.IO.File.Exists(FilePath))
                    {
                        var ResponseSaved = new DeleteFileResponse("File Delete Failed", FilePath);
                        DeleteFileError(this, ResponseSaved);
                        DeleteCompletionSource.TrySetResult(ResponseSaved);
                    }
                    else
                    {
                        var ResponseSaved = new DeleteFileResponse("File Delete successfully", FilePath);
                        DeleteFileSuccess(this, ResponseSaved);
                        DeleteCompletionSource.TrySetResult(ResponseSaved);
                    }
                }
                else
                {
                    var ResponseSaved = new DeleteFileResponse("File Path Not Exist", FilePath);
                    DeleteFileError(this, ResponseSaved);
                    DeleteCompletionSource.TrySetResult(ResponseSaved);
                }
            }
            else
            {
                var ResponseSaved = new DeleteFileResponse("Please Check your File Path", null);
                DeleteFileError(this, ResponseSaved);
                DeleteCompletionSource.TrySetResult(ResponseSaved);
            }
            return await DeleteCompletionSource.Task;
        }

        public async Task<SaveFileResponse> CreateThumbnail(string FilePath, string DestFilePath, float ImgWidth = 0, float ImgHight = 0, int ImgQuality = 0)
        {
            if (!string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(DestFilePath))
            {

                if (System.IO.File.Exists(FilePath))
                {
                    byte[] array = System.IO.File.ReadAllBytes(FilePath);
                    return await CreateThumbnail(array, DestFilePath, ImgWidth, ImgHight, ImgQuality);
                }
                else
                {
                    var ResponseSaved = new SaveFileResponse("Source File Path Does Not Exist", null, null);
                    SaveFileError(this, ResponseSaved);
                    SaveCompletionSource.TrySetResult(ResponseSaved);
                }
            }
            else
            {
                var ResponseSaved = new SaveFileResponse("Please check your File Path and Destination Path", null, null);
                SaveFileError(this, ResponseSaved);
                SaveCompletionSource.TrySetResult(ResponseSaved);
            }
            return await SaveCompletionSource.Task;
        }

        public async Task<SaveFileResponse> CreateThumbnail(byte[] ImageData, string DestFilePath, float ImgWidth = 0, float ImgHight = 0, int ImgQuality = 0)
        {
            if (!string.IsNullOrEmpty(DestFilePath) && ImageData.Length == 0 && ImageData == null)
            {
                // Load the bitmap
                Bitmap originalImage = BitmapFactory.DecodeByteArray(ImageData, 0, ImageData.Length);

                float oldWidth = (float)originalImage.Width;
                float oldHeight = (float)originalImage.Height;
                float scaleFactor = 0f;

                if (ImgWidth > 0 && ImgHight > 0 && ImgQuality > 0)
                {
                    if (oldWidth > oldHeight)
                    {
                        scaleFactor = ImgWidth / oldWidth;
                    }
                    else
                    {
                        scaleFactor = ImgHight / oldHeight;
                    }

                    float newHeight = oldHeight * scaleFactor;
                    float newWidth = oldWidth * scaleFactor;

                    Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        resizedImage.Compress(Bitmap.CompressFormat.Jpeg, ImgQuality, ms);
                        byte[] array = ms.ToArray();
                        return await SaveFile(array, DestFilePath);
                    }

                }
                else
                {
                    float width = 500;
                    float height = 400;
                    int Quality = 50;
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

                    Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);

                    using (MemoryStream ms = new MemoryStream())
                    {
                        resizedImage.Compress(Bitmap.CompressFormat.Jpeg, Quality, ms);
                        byte[] array = ms.ToArray();
                        return await SaveFile(array, DestFilePath);
                    }
                }
            }
            else
            {
                var ResponseSaved = new SaveFileResponse("Please check your Destination File Path and Image Data", null, null);
                SaveFileError(this, ResponseSaved);
                SaveCompletionSource.TrySetResult(ResponseSaved);

            }
            return await SaveCompletionSource.Task;
        }

        #region "Api Actions"

        //PostRequestAsync function
        public async Task<ApiResponseResult<T>> PostRequestAsync<T>(string ApiUrl, IDictionary<string, string> Parameters, IDictionary<string, string> Headers = null)
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
            catch (OperationCanceledException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException)
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
        public async Task<T> GetRequestAsync<T>(string ApiUrl, IDictionary<string, string> Parameters = null, IDictionary<string, string> Headers = null)
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
            catch (OperationCanceledException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException)
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
            catch (OperationCanceledException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException)
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
        //Deserialization of Json Object
        private T Deserialize<T>(string result)
        {
            var toReturn = JsonConvert.DeserializeObject<T>(result);
            return toReturn;
        }
        #endregion

        #region "CreateLogFile"

        public async Task<SaveFileResponse> CreateLogFile(string FileName, string FilePath, string ExtentionForFile)
        {
            SaveCompletionSource = new TaskCompletionSource<SaveFileResponse>();
            try
            {
                if (!string.IsNullOrEmpty(FileName) && !string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(ExtentionForFile))
                {

                    if (!Directory.Exists(FilePath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(FilePath);
                    }

                    var LogFilePath = System.IO.Path.Combine(FilePath, FileName.ToString() + $".{ExtentionForFile}");
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
                        var ResponseSaved = new SaveFileResponse("Extention is null", null, null);
                        SaveFileError(this, ResponseSaved);
                        SaveCompletionSource.TrySetResult(ResponseSaved);
                    }
                }
            }
            catch (Java.IO.IOException ex)
            {
                var ResponseSaved = new SaveFileResponse(ex.ToString(), FileName, null);
                SaveFileError(this, ResponseSaved);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                SaveCompletionSource.TrySetResult(ResponseSaved);
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
                ActionLoggerDirectory = System.IO.Path.Combine(ProvidedactionLoggerDirectory, Foldername); ;
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
            if (System.IO.File.Exists(LogFilePath))
            {
                try
                {
                    System.IO.File.AppendAllLines(LogFilePath, _theUserActions);
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
                    System.IO.File.WriteAllLines(LogFilePath, _theUserActions);
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
                if (System.IO.File.Exists(file))
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddDays(-Days))
                        fi.Delete();
                }
            }
            return await Task.FromResult(files);
        }

        public Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality)
        {
            // Load the bitmap
            Bitmap originalImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);

            float oldWidth = (float)originalImage.Width;
            float oldHeight = (float)originalImage.Height;
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

            Bitmap resizedImage = Bitmap.CreateScaledBitmap(originalImage, (int)newWidth, (int)newHeight, false);
            if (originalImage != null)
            {
                originalImage.Recycle();
                originalImage = null;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                if (resizedImage != null)
                {
                    resizedImage.Recycle();
                    resizedImage = null;
                }
                return Task.FromResult(ms.ToArray());
            }
        }


        #endregion

        public Task<bool> LogServiceAsync(string FilePath, string FileName, Dictionary<string, string> UserData)
        {
            bool isLogDone = false;
            try
            {
                if (!string.IsNullOrEmpty(FilePath) && !string.IsNullOrEmpty(FileName) && UserData != null)
                {
                    string LogFilename = $"{FileName}_{DateTime.Now.ToString("yyyy-MM-dd")}.txt";

                    if (!Directory.Exists(FilePath))
                    {
                        DirectoryInfo di = Directory.CreateDirectory(FilePath);
                    }


                    string NewFilePath = System.IO.Path.Combine(FilePath, LogFilename);

                    if (!System.IO.File.Exists(NewFilePath))
                    {
                        System.IO.File.Create(NewFilePath);

                    }


                    using (StreamWriter file = new StreamWriter(NewFilePath))
                        for (int i = 0; i <= UserData.Count; i++)
                        {
                            foreach (var entry in UserData)
                            {
                                file.Write("{0} \t", entry.Value.ToString());
                            }
                            if (i == UserData.Count)
                            {
                                file.Write("\n");
                                file.Close();

                            }
                        }
                    isLogDone = true;

                    return Task.FromResult(isLogDone);

                }
                else
                {
                    return Task.FromResult(isLogDone);
                }
            }
            catch (Exception)
            {
                return Task.FromResult(isLogDone);

            }
        }

        // Getting User Data in  List
        public Task<bool> LogServiceAsync(string FilePath, string FileName, List<string> UserData)
        {
            //Converting List into  IDict Key Value Pair
            var res = UserData.ToDictionary(x => x, x => string.Format(x.ToString()));
            return LogServiceAsync(FilePath, FileName, res);

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
            catch (OperationCanceledException)
            {
                errorResponse.Status = "-2";
                errorResponse.Message = "Please check your internet connection, and try again later.";
                var resString = JsonConvert.SerializeObject(errorResponse);
                response = Deserialize<T>(resString);
            }
            catch (JsonReaderException)
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

        public Task<T> GetRequest<T>(string ApiUrl, IDictionary<string, string> Parameters = null, IDictionary<string, string> Headers = null)
        {
            throw new NotImplementedException();
        }

        public class CountingRequestBody : RequestBody
        {
            protected RequestBody _body;
            protected ICountProgressListener _listener;
            protected string _tag;
            protected CountingSink countingSink;

            public CountingRequestBody(RequestBody body, string file_name, ICountProgressListener listener)
            {
                _body = body;
                _tag = file_name;
                _listener = listener;
            }
            public override MediaType ContentType()
            {
                return _body.ContentType();
            }
            public override long ContentLength()
            {
                return _body.ContentLength();
            }

            public override void WriteTo(IBufferedSink p0)
            {

                try
                {
                    IBufferedSink bufferedSink;
                    countingSink = new CountingSink(this, p0);
                    bufferedSink = Okio.Buffer(countingSink);

                    _body.WriteTo(bufferedSink);

                    bufferedSink.Flush();
                }
                catch (Java.IO.IOException ex)
                {
                    _listener?.OnError(_tag, ex.ToString());
                }


            }

            public class CountingSink : ForwardingSink
            {
                private long bytesWritten = 0;
                CountingRequestBody _parent;

                public CountingSink(CountingRequestBody parent, ISink sink) : base(sink)
                {
                    _parent = parent;
                }

                public override void Write(OkBuffer p0, long p1)
                {
                    try
                    {
                        base.Write(p0, p1);

                        bytesWritten += p1;
                        _parent?._listener.OnProgress(_parent._tag, bytesWritten, _parent.ContentLength());
                    }
                    catch (Java.IO.IOException ex)
                    {
                        _parent?._listener?.OnError(_parent._tag, ex.ToString());
                    }

                }


            }
        }
    }
}
