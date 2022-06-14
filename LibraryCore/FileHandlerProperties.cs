using System.Collections.Generic;
using System.IO;

namespace LibraryCore
{
    /// <summary>
    /// Value for FileBytesItem
    /// </summary>
    /// <value >Property <c>Name</c> gets the value of the string field, File Name</value> 
    /// <value> Property <c>Bytes</c> gets the value of the byte field, Bytes<</value> 
    public class FileBytesItem
    {
        public string Name { get; }
        public string FileType { get; }
        public byte[] Bytes { get; }

        public FileBytesItem(string fileType, byte[] bytes, string name)
        {
            Name = name;
            Bytes = bytes;
            FileType = fileType;
        }
    }

    /// <summary>
    /// Value for FilePathItem
    /// </summary>
    /// <value> Property <c>Path</c> gets the value of the string field, File Path</value> 
    public class FilePathItem
    {
        public string FileName { get; }
        public string FileRelPath { get; }
        public string File { get; }
        public string FileType { get; }

        public FilePathItem(string fileName, string fileRelPath, string fileType, string file)
        {
            FileName = fileName;
            FileRelPath = fileRelPath;
            File = file;
            FileType = fileType;
        }
    }

    /// <summary>
    /// Value for FileUploadProgress
    /// </summary>
    /// <value> Property <c>TotalBytesSent</c> gets the value of the long field, TotalBytesSent</value> 
    /// <value> Property <c>TotalLength</c> gets the value of the long field, TotalLength</value> 
    /// <value> Property <c>Percentage</c> gets the value of the double field, Percentage</value> 
    /// <value> Property <c>FileName</c> gets the value of the string field, FileName</value> 
    public class FileUploadProgress
    {
        public long TotalBytesSent { get; }
        public long TotalLength { get; }
        public double Percentage { get { return TotalLength > 0 ? 100.0f * ((double)TotalBytesSent / (double)TotalLength) : 0.0f; } }
        public string FileName { get; }

        public FileUploadProgress(long totalBytesSent, long totalLength, string file_name)
        {
            TotalBytesSent = totalBytesSent;
            TotalLength = totalLength;
            FileName = Path.GetFileName(file_name);
        }

    }

    /// <summary>
    /// Value for FileUploadResponse
    /// </summary>
    /// <value> Property <c>FileName</c> gets the value of the string field, FileName</value> 
    /// <value> Property <c>Result</c> gets the value of the string field, API Result</value> 
    /// <value> Property <c>StatusCode</c> gets the value of the int field, API response StatusCode</value> 
    /// <value> Property <c>Headers</c> gets the value of the string field, Headers</value> 
    public class FileUploadResponse
    {
        public string FileName { get; }
        public string Result { get; }
        public int StatusCode { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }

        public FileUploadResponse(string result, int statuCode, string file_name, IReadOnlyDictionary<string, string> headers)
        {
            Result = result;
            StatusCode = statuCode;
            FileName = Path.GetFileName(file_name);
            Headers = headers;
        }
    }

    //public class ExecptionHandler
    //{
    //    public string FunctionName { get; }
    //    public string Exception { get; }
    //    public string Details { get; }
    //    public string Info { get; }

    //    public ExecptionHandler(string functionName, string exception, string details, string info)
    //    {
    //        FunctionName = functionName;
    //        Exception = exception;
    //        Details = details;
    //        Info = info;
    //    }
    //}

    /// <summary>
    /// Value for UploadFileItemInfo
    /// </summary>
    /// <value> Property <c>Data</c> gets the value of the string field, FileName</value> 
    /// <value> Property <c>FileName</c> gets the value of the string field, FileName</value> 
    /// <value> Property <c>OriginalPath</c> gets the value of the string field, OriginalPath</value> 
    /// <value> Property <c>IsTemporal</c> gets the value of the bool field, IsTemporal</value> 
    public class UploadFileItemInfo
    {
        public byte[] Data { get; set; }
        public string FileName { get; }
        public string OriginalPath { get; }

        public bool IsTemporal { get; }

        public UploadFileItemInfo(byte[] data, string fileName)
        {
            Data = data;
            FileName = fileName;
        }

        public UploadFileItemInfo(string originalPath, string fileName, bool isTemporal)
        {
            OriginalPath = originalPath;
            FileName = fileName;
            IsTemporal = isTemporal;
        }
    }

    public class SaveFileResponse
    {
        public string Result { get; }
        public string FilePath { get; }
        public string FileName { get; }

        public SaveFileResponse(string result, string originalPath, string fileName)
        {
            Result = result;
            FilePath = originalPath;
            FileName = fileName;

        }
    }

    public class ApiResponseResult<T>
    {
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public List<ErrorViewModel> Errors { get; set; }
        public T Result { get; set; }

        public ApiResponseResult()
        {
            Result = default(T);
            Errors = new List<ErrorViewModel>();
            Status = null;
        }
    }

    public class ErrorViewModel
    {
        public string Key { get; set; }
        public string Message { get; set; }
    }

    public class DeleteFileResponse
    {
        public string Result { get; }
        public string FilePath { get; }

        public DeleteFileResponse(string result, string originalPath)
        {
            Result = result;
            FilePath = originalPath;
        }
    }

    public class GetBytesResponse
    {
        public string Result { get; }
        public byte[] FileBytes { get; set; }

        public GetBytesResponse(string result, byte[] array)
        {
            Result = result;
            FileBytes = array;
        }
    }

    public class CreateDbConnection
    {
        public string Result { get; }
        public string ConnctionString { get; }

        public CreateDbConnection(string result, string databasePath)
        {
            Result = result;
            ConnctionString = databasePath;
        }
    }
}