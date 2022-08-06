using Library_Mangement.Helper;
using Library_Mangement.Model.ApiResponse;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Services
{
    public class ExceptionHandlerService
    {
        public static void SendErrorLog(string moduleName, Exception ex, object methodParams = null,
              [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
              [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
              [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0, string extraMessage = "")
        {
            try
            {
                Task.Run(async () => await UploadExceptionDetails(moduleName, ex, methodParams, memberName, sourceFilePath, sourceLineNumber, extraMessage));
            }
            catch (Exception error)
            {
                System.Diagnostics.Debug.WriteLine($"ExceptionService Method : Error :: {error.StackTrace}");
            }
        }

        private static async Task UploadExceptionDetails(string moduleName, Exception ex, object methodParams,
                string memberName, string sourceFilePath, int sourceLineNumber, string extraMessage)
        {
            //var deviceInfo = DependencyService.Get<IDeviceInfo>();
            var deviceInfo = Common.DeviceDetails();
            string buildMode = Common.GetBuildMode;// "DEV", "STAGE", "PROD"
            var logModel = new LogModel();
            logModel.FileName = moduleName;
            logModel.AppName = AppConfig.LogType_Error;
            logModel.Source = $"OCLM_XamarinApp_{buildMode}";
            logModel.Module = moduleName;
            if (string.IsNullOrEmpty(extraMessage))
                logModel.Method = memberName;
            else
                logModel.Method = memberName + " " + extraMessage;

            logModel.MethodParams = methodParams == null ? "" : Common.JsonConvertSerializeObject(methodParams);
            string loginUser = "";
            if (App.CurrentLoggedInUser != null)
            {
                loginUser = $"AdvisorId - {App.CurrentLoggedInUser.StudentId}";
            }
            logModel.DeviceName = deviceInfo.DeviceModel;
            logModel.AndroidLevel = $"{deviceInfo.Platform} | {deviceInfo.OSVersion} ";
            logModel.NetworkProvider = deviceInfo.NetworkOperatorName;
            logModel.BuildNumber = deviceInfo.BuildNumber;
            logModel.DeviceManufacturer = deviceInfo.DeviceManufacturer;
            logModel.Logdate = DateTime.Now;
            if (ex != null)
            {
                int idx = 2;
                string inExDets = string.Empty;
                while (ex.InnerException != null)
                {
                    inExDets += "<br />" + idx + "<br />" + ex.InnerException.ToString();
                    idx++;
                    ex = ex.InnerException;
                }
                logModel.SubMessage = inExDets;
                logModel.ExtraInfo = ex.StackTrace;
                logModel.Message = ex.Message;
            }
            //await App.LogDatabase.Log.AddExceptionLogs(logModel);
            var networkFlag = CrossConnectivity.Current.IsConnected;
            if (networkFlag)
            {
                //var resp = await RestService.SendExceptionAsync(logModel);
            }
        }
    }
}
