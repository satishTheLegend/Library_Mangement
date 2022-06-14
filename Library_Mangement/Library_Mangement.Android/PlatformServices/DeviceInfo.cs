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
        Context CurrentContext => CrossCurrentActivity.Current.Activity;
        string id = string.Empty;
        public string DeviceID
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(id))
                    return id;

                id = Build.Serial;
                if (id == Build.Unknown)
                {
                    try
                    {
                        id = Secure.GetString(Forms.Context.ContentResolver, Secure.AndroidId);
                    }
                    catch (Exception ex)
                    {
                        Log.Warn("DeviceInfo", "Unable to get id: " + ex);
                    }
                }
                return id;
            }
        }
        public string PackageName
        {
            get
            {
                return Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).PackageName;
            }
        }

        [Obsolete("Obsolete")]
        public string GetPhoneNumber
        {
            get
            {
                var mTelephonyMgr = (TelephonyManager)Forms.Context.GetSystemService(Context.TelephonyService);
                var number = mTelephonyMgr?.Line1Number;
                return number;
            }
        }

        [Obsolete("Obsolete")]
        public string Version => Forms.Context.PackageManager?.GetPackageInfo(Forms.Context.PackageName, 0)?.VersionName;

        public string OSVersion => Build.VERSION.Release;

        public string Model => Build.Model;

        public string AppName => Forms.Context.PackageManager.GetPackageInfo(Forms.Context.PackageName, 0).ApplicationInfo.LabelRes.ToString();

        public string Platform => "android";

        [Obsolete("Obsolete")]
        public string BuildNumber
        {
            get
            {
                int build = CurrentContext.PackageManager.GetPackageInfo(CurrentContext.PackageName, 0).VersionCode;
                return build.ToString();
            }
        }
        [Obsolete("Obsolete")]
        public string GetIdentifier
        {
            get
            {
                var mTelephonyMgr = (TelephonyManager)Forms.Context.GetSystemService(Context.TelephonyService);
                return mTelephonyMgr?.DeviceId;
            }
        }

        public string GetDeviceID
        {
            get
            {
                var context = Application.Context;
                string deviceId = Secure.GetString(context.ContentResolver, Secure.AndroidId);
                return deviceId;
            }
        }

        public string GetAppVersion
        {
            get
            {
                var context = Application.Context;
                var versionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
                return versionNumber;
            }
        }

        [Obsolete("Obsolete")]
        public string GetNetworkOperatorName
        {
            get
            {
                var mTelephonyMgr = (TelephonyManager)Forms.Context.GetSystemService(Context.TelephonyService);
                return mTelephonyMgr?.NetworkOperatorName;
            }
        }
        public string DeviceName => Build.Manufacturer;
    }
}