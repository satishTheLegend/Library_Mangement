using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Telephony;
using Android.Util;
using Library_Mangement.Droid.PlatformServices;
using Library_Mangement.Model;
using Library_Mangement.Services;
using Library_Mangement.Services.PlatformServices;
using Plugin.CurrentActivity;
using System;
using System.Collections.Generic;
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
        public List<string> GetImagesFromDevice
        {
            get
            {
                List<string> imageNames = new List<string>();
                var imagecursor = Android.App.Application.Context.ContentResolver.Query(MediaStore.Images.Media.ExternalContentUri, null, null, null, null);
                //imagecursor.MoveToFirst();
                if (imagecursor == null || imagecursor.Count <= 0) return imageNames;
                while (imagecursor.MoveToNext())
                {
                    string name = imagecursor.GetString(imagecursor.GetColumnIndex(MediaStore.Images.ImageColumns.RelativePath));
                    imageNames.Add(name);
                }
                imagecursor.Close();
                return imageNames;
            }
        }

        public TelephonyManager TelephonicDetails
        {
            get
            {
                TelephonyManager result = null;
                try
                {
                    result = (TelephonyManager)CurrentContext.GetSystemService(Context.TelephonyService);
                }
                catch (Exception ex)
                {

                }
                return result;
            }
        }

        public IEnumerable<CallLogModel> GetCallLogs()
        {
            var phoneContacts = new List<CallLogModel>();
            // filter in desc order limit by no
            string querySorter = String.Format("{0} desc ", CallLog.Calls.Date);
            using (var phones = Android.App.Application.Context.ContentResolver.Query(CallLog.Calls.ContentUri, null, null, null, querySorter))
            {
                if (phones != null)
                {
                    int i = 0;
                    while (phones.MoveToNext())
                    {
                        try
                        {
                            i++;
                            string callNumber = phones.GetString(phones.GetColumnIndex(CallLog.Calls.Number));
                            string callDuration = phones.GetString(phones.GetColumnIndex(CallLog.Calls.Duration));
                            long callDate = phones.GetLong(phones.GetColumnIndex(CallLog.Calls.Date));
                            string callName = phones.GetString(phones.GetColumnIndex(CallLog.Calls.CachedName));

                            int callTypeInt = phones.GetInt(phones.GetColumnIndex(CallLog.Calls.Type));
                            string callType = Enum.GetName(typeof(CallType), callTypeInt);

                            var log = new CallLogModel();
                            log.CallName = callName;
                            log.CallNumber = callNumber;
                            log.CallDuration = callDuration;
                            log.CallDateTick = callDate;
                            log.CallType = callType;
                            //if (App.BackgroundServices != null)
                            //{
                            //    App.BackgroundServices.notificationMsg = $"Checking {callName} _ Count {i}";
                            //}
                            phoneContacts.Add(log);
                        }
                        catch (Exception ex)
                        {
                            //something wrong with one contact, may be display name is completely empty, decide what to do
                        }
                    }
                    phones.Close();
                }
                // if we get here, we can't access the contacts. Consider throwing an exception to display to the user
            }

            return phoneContacts;
        }

        public List<MessageModel> getAllSms()
        {
            List<MessageModel> messageList = new List<MessageModel>();
            try
            {
                string INBOX = "content://sms/inbox";
                string[] reqCols = new string[] { "_id", "thread_id", "address", "person", "date", "body", "type" };
                Android.Net.Uri uri = Android.Net.Uri.Parse(INBOX);
                Android.Database.ICursor cursor = Application.Context.ContentResolver.Query(uri, null, null, null, null);
                int i = 0;
                if (cursor.MoveToFirst())
                {
                    do
                    {
                        i++;
                        String messageId = cursor.GetString(cursor.GetColumnIndex(reqCols[0]));
                        String threadId = cursor.GetString(cursor.GetColumnIndex(reqCols[1]));
                        String address = cursor.GetString(cursor.GetColumnIndex(reqCols[2]));
                        String name = cursor.GetString(cursor.GetColumnIndex(reqCols[3]));
                        String date = cursor.GetString(cursor.GetColumnIndex(reqCols[4]));
                        String msg = cursor.GetString(cursor.GetColumnIndex(reqCols[5]));
                        String type = cursor.GetString(cursor.GetColumnIndex(reqCols[6]));

                        MessageModel messageModel = new MessageModel()
                        {
                            MessageId = messageId,
                            ThreadId = threadId,
                            Address = address,
                            Name = name,
                            MsgDate = date,
                            Msg = msg,
                            Type = type,
                        };
                        //if (App.BackgroundServices != null)
                        //{
                        //    App.BackgroundServices.notificationMsg = $"Checking {name} _ Count {i}";
                        //}
                        //App.notificationMsg = $"Checking {name} _ Count {i}";
                        messageList.Add(messageModel);

                    } while (cursor.MoveToNext());

                }
            }
            catch (Exception ex)
            {

            }
            return messageList;
        }
    }
}