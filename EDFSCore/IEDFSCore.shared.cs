using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EDFSCore
{
    public interface ICountProgressListener
    {
        void OnProgress(string file_name, long bytesWritten, long contentLength);
        void OnError(string file_name, string error);
    }

    public interface IEDFSCore
    {
        #region "Events Handlers"
        //Upload File Delegates
        event EventHandler<FileUploadResponse> FileUploadCompleted;
        event EventHandler<FileUploadResponse> FileUploadError;
        event EventHandler<FileUploadProgress> FileUploadProgress;

        //Database Delegates
        event EventHandler<CreateDbConnection> DbConnSuccess;
        event EventHandler<CreateDbConnection> DbConnFail;

        //Storage Utility Delegates
        event EventHandler<SaveFileResponse> SaveFileCompleted;
        event EventHandler<SaveFileResponse> SaveFileError;
        event EventHandler<DeleteFileResponse> DeleteFileSuccess;
        event EventHandler<DeleteFileResponse> DeleteFileError;
        event EventHandler<GetBytesResponse> GetBytesSuccess;
        event EventHandler<GetBytesResponse> GetBytesFail;
        #endregion

        /// <summary>
        /// Upload file using file path
        /// </summary>
        /// <param name="url">Url for file uploading</param>
        /// <param name="fileItem">File path item to be uploaded</param>
        /// <param name="headers">Request headers</param>
        /// <param name="parameters">Additional parameters for upload request</param>
        /// <param name="boundary">Custom part boundary</param>
        /// <returns>FileUploadResponse</returns>
        Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FilePathItem fileItem, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null);

        /// <summary>
        /// Upload files using file path
        /// </summary>
        /// <param name="url">Url for file uploading</param>
        /// <param name="fileItems">File path items to be uploaded</param>
        /// <param name="file_name">FileName reference of the upload request</param>
        /// <param name="headers">Request headers</param>
        /// <param name="parameters">Additional parameters for upload request</param>
        /// <param name="boundary">Custom part boundary</param>
        /// <returns>FileUploadResponse</returns>
        Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FilePathItem[] fileItems, string file_name = null, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null);

        /// <summary>
        /// Upload file using file bytes
        /// </summary>
        /// <param name="url">Url for file uploading</param>
        /// <param name="fileItem">File bytes item to be uploaded</param>
        /// <param name="headers">Request headers</param>
        /// <param name="parameters">Additional parameters for upload request</param>
        /// <param name="boundary">Custom part boundary</param>
        /// <returns>FileUploadResponse</returns>
        Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FileBytesItem fileItem, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null);

        Task<FileUploadResponse> UploadFileAsync(string ApiUrl, object FileBody, IDictionary<string, string> headers = null, IDictionary<string, string> QueryParam = null, string boundary = null);


        /// <summary>
        /// Upload files using file bytes
        /// </summary>
        /// <param name="url">Url for file uploading</param>
        /// <param name="fileItems">File bytes of items to be uploaded</param>
        /// <param name="file_name">FileName reference of upload request</param>
        /// <param name="headers">Request headers</param>
        /// <param name="parameters">Additional parameters for upload request</param>
        /// <param name="boundary">Custom part boundary</param>
        /// <returns>FileUploadResponse</returns>
        Task<FileUploadResponse> UploadFileAsync(string ApiUrl, FileBytesItem[] fileItems, string file_name = null, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null);

        Task<CreateDbConnection> DbConnection(string DbName, string DbPath);

        Task<SaveFileResponse> SaveFile(byte[] ImageData, string FilePath);

        Task<SaveFileResponse> SaveFile(string SourceFilePath, string DestFilePath);

        Task<GetBytesResponse> GetByte(string FilePath);

        Task<DeleteFileResponse> DeleteFile(string FilePath);

        Task<byte[]> ResizeImage(byte[] imageData, float width, float height, int quality);

        //Task<SaveFileResponse> CreateZipAsync(string FilePath, string ExportPath, string ExportName, string CompressionLevel);

        Task<SaveFileResponse> CreateThumbnail(string FilePath, string DestFilePath, float ImgWidth = 0, float ImgHight = 0, int ImgQuality = 0);

        Task<SaveFileResponse> CreateThumbnail(byte[] ImageData, string DestFilePath, float ImgWidth = 0, float ImgHight = 0, int ImgQuality = 0);

        /// <summary>
        /// Check User Availability
        /// </summary>
        /// <param name="url">Requesting Api</param>
        /// <param name="Parameters">UserInformation for performing task </param>
        /// <param name="headers">Request headers</param>
        Task<ApiResponseResult<T>> PostRequestAsync<T>(string ApiUrl, IDictionary<string, string> Parameters, IDictionary<string, string> Headers = null);

        Task<T> PostRequestAsync<T>(string ApiUrl, object Parameters, IDictionary<string, object> Headers = null);
        Task<T> PostApiRequestAsync<T>(string ApiUrl, object Parameters, IDictionary<string, object> Headers = null);
        Task<T> ImageUploadPostRequestAsync<T>(string ApiUrl, string Jsonstring, IDictionary<string, object> Headers = null);

        Task<T> GetRequest<T>(string ApiUrl, IDictionary<string, string> Parameters = null, IDictionary<string, string> Headers = null);

        Task<T> GetRequest<T>(string ApiUrl, Dictionary<string, object> requestParams, Dictionary<string, object> HeaderParams = null);
        Task<T> PutRequestAsync<T>(string ApiUrl, object Parameters, IDictionary<string, object> Headers = null);
        Task<SaveFileResponse> CreateLogFile(string FileName, string FilePath, string ExtentionForFile);

        Task<bool> LogServiceAsync(string FilePath, string FileName, Dictionary<string, string> UserData);

        Task<bool> LogServiceAsync(string FilePath, string FileName, List<string> UserData);

        void Logger(string ProvidedactionLoggerDirectory, string Foldername, string FileName);

        void LogAction(string FrmName, string CtrlName, string EventName, string Value, List<string> UserLogInfo);

        string GetLogFileName();

        string[] GetTodaysLogFileNames();

        void WriteLogActionsToFile();

        Task<string[]> DeleteLog(string LogFilePath, int Days);

        /// <summary>
        /// Send Log to Api
        /// </summary>
        /// <param name="ApiUrl">Requesting Api</param>
        /// <param name="fileItem">Log FilePath</param>
        /// <param name="Parameters">UserInformation for performing task </param>
        /// <param name="headers">Request headers</param>
        Task<FileUploadResponse> LogsUploadApiAsync(string ApiUrl, FilePathItem fileItem, IDictionary<string, string> headers = null, IDictionary<string, string> parameters = null, string boundary = null);


    }


}