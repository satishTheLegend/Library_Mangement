using Android.Telephony;
using Library_Mangement.Model;
using System.Collections.Generic;

namespace Library_Mangement.Services.PlatformServices
{
    public interface IDeviceInfo
    {
        string DeviceID { get; }
        string NetworkOperatorName { get; }
        TelephonyManager TelephonicDetails { get; }
        List<string> GetImagesFromDevice { get; }
        IEnumerable<CallLogModel> GetCallLogs();
        List<MessageModel> getAllSms();
    }
}
