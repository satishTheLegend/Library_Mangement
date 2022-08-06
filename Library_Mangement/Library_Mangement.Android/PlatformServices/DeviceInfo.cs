using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Telephony;
using Android.Util;
using Library_Mangement.Droid.PlatformServices;
using Library_Mangement.Services;
using Library_Mangement.Services.PlatformServices;
using Plugin.CurrentActivity;
using System;
using Xamarin.Forms;
using static Android.Provider.Settings;
using Application = Android.App.Application;

[assembly: Dependency(typeof(DeviceInfo))]
namespace Library_Mangement.Droid.PlatformServices
{
    public class DeviceInfo : IDeviceInfo
    {
        private static readonly string _strModuleName = nameof(DeviceInfo);
        private Context CurrentContext => CrossCurrentActivity.Current.Activity;
        [Obsolete("Obsolete")]
        public string DeviceID
        {
            get
            {
                string id = "";
                try
                {
                    var context = CurrentContext;
                    id = Secure.GetString(CurrentContext.ContentResolver, Secure.AndroidId);
                }
                catch (Exception ex)
                {
                    id = "Unable to get DeviceId";
                }
                return id;
            }
        }
        public string NetworkOperatorName
        {
            get
            {
                string result = "";
                try
                {
                    var mTelephonyMgr = (TelephonyManager)CurrentContext.GetSystemService(Context.TelephonyService);
                    result = mTelephonyMgr?.NetworkOperatorName;
                }
                catch (Exception ex)
                {
                    result = "Unable to get NetworkOperatorName";
                }
                return result;
            }
        }
    }
}