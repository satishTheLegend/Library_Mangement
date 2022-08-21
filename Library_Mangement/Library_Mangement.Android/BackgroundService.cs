
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Library_Mangement.Helper;
using Library_Mangement.Services;
using System;
using System.Threading.Tasks;

namespace Library_Mangement.Droid
{
    [Service(Enabled = true)]
    public class BackgroundService : Android.App.Service
    {
        #region Proerties
        private bool isStarted;
        NotificationCompat.Builder notifBuilder = null;
        private Handler handler;
        private string msg = "Library is setting up things";
        private Action runnable;
        public const long INTERVAL = 100;
        private NotificationManager _notificationManager = null;
        Notification notification = null;
        private static string foregroundChannelId = "9001";
        private static Context context = Application.Context;
        public const int SERVICE_RUNNING_NOTIFICATION_ID = 10001;
        private readonly string _strModuleName = nameof(BackgroundService);
        #endregion

        #region override Methods
        public override void OnCreate()
        {
            base.OnCreate();
            handler = new Handler();
            runnable = () =>
            {
                try
                {
                    if (isStarted)
                    {
                        Task.Run(async () =>
                        {
                            App.BackgroundServices = new BackgroundServices();
                            if (!App.BackgroundServices.IsServiceBusy)
                            {
                                App.BackgroundServices.IsServiceBusy = true;
                                StopSelf();
                            }
                        });
                    }
                }
                catch (Exception ex)
                {

                }
            };
        }
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            try
            {
                if (!isStarted)
                {
                    isStarted = true;
                    notification = ReturnNotification();
                    StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
                    handler.PostDelayed(runnable, INTERVAL);
                }
            }
            catch (Exception ex)
            {
                isStarted = false;
            }
            // This tells Android restart the service if it is killed to reclaim resources.
            return StartCommandResult.Sticky;
        }
        public override void OnDestroy()
        {
            isStarted = false;
            StopForeground(true);
            handler.RemoveCallbacks(runnable);
            DismissNotification(SERVICE_RUNNING_NOTIFICATION_ID);
            base.OnDestroy();
        }
        public override void OnTaskRemoved(Intent rootIntent)
        {
            base.OnTaskRemoved(rootIntent);
        }
        #endregion

        #region Private Methods
        private void DismissNotification(int nId)
        {
            if (_notificationManager != null)
            {
                _notificationManager.Cancel(nId);
            }
        }
        private Notification ReturnNotification()
        {
            // Building intent
            
            try
            {
                
                var intent = new Intent(this, typeof(MainActivity));
                
                intent.AddFlags(ActivityFlags.SingleTop);
                intent.PutExtra("Title", "Title");

                var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.Mutable); 
                notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
                    .SetContentTitle("Library Checking For New Data")
                    .SetSmallIcon(Resource.Mipmap.icon)
                    .SetOngoing(true)
                    .SetProgress(100, 100, true)
                    .SetContentText(msg)
                    .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.borrow_books))
                    //.SetColor(Color.Rgb(92,163,4))
                    .SetContentIntent(pendingIntent);

                // Building channel if API verion is 26 or above
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                {
                    NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, "Title", NotificationImportance.High);
                    notificationChannel.Importance = NotificationImportance.Default;
                    notificationChannel.EnableLights(true);
                    notificationChannel.EnableVibration(false);
                    notificationChannel.ShouldVibrate();
                    notificationChannel.SetShowBadge(true);
                    notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

                    _notificationManager = context.GetSystemService(NotificationService) as NotificationManager;
                    if (_notificationManager != null)
                    {
                        notifBuilder.SetChannelId(foregroundChannelId);
                        _notificationManager.CreateNotificationChannel(notificationChannel);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return notifBuilder.Build();
        }

        //private void Init_SetForegroundServiceNotification()
        //{
        //    try
        //    {
        //        // Building intent
        //        var intent = new Intent(this, typeof(MainActivity));
        //        intent.AddFlags(ActivityFlags.SingleTop);
        //        intent.PutExtra(AppResources.Background_Service_Title, AppResources.Background_Service_Title_Msg);

        //        var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);

        //        var notifBuilder = new NotificationCompat.Builder(context, foregroundChannelId)
        //            .SetContentTitle(AppResources.Background_Service_Title)
        //            .SetSmallIcon(Resource.Mipmap.icon)
        //            .SetOngoing(true)
        //            .SetProgress(100, 100, true)
        //            .SetContentText(AppResources.Background_Service_Msg)
        //            .SetContentIntent(pendingIntent);


        //        // Building channel if API verion is 26 or above
        //        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        //        {
        //            NotificationChannel notificationChannel = new NotificationChannel(foregroundChannelId, AppResources.Background_Service_Title, NotificationImportance.High);
        //            notificationChannel.Importance = NotificationImportance.High;
        //            notificationChannel.EnableLights(true);
        //            notificationChannel.EnableVibration(false);
        //            //notificationChannel.SetShowBadge(true);
        //            //notificationChannel.SetVibrationPattern(new long[] { 100, 200, 300, 400, 500, 400, 300, 200, 400 });

        //            _notificationManager = context.GetSystemService(NotificationService) as NotificationManager;
        //            if (_notificationManager != null)
        //            {
        //                notifBuilder.SetChannelId(foregroundChannelId);
        //                _notificationManager.CreateNotificationChannel(notificationChannel);
        //            }
        //        }
        //        else
        //        {
        //            notification = new Notification.Builder(this)
        //                .SetContentTitle(AppResources.Background_Service_Title)
        //                .SetContentText("")
        //                .SetSmallIcon(Resource.Mipmap.icon)
        //                .SetOngoing(true)
        //                .Build();
        //        }

        //        var forGroundNotification = notifBuilder.Build();
        //        StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, forGroundNotification);
        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionHandlerService.SendErrorLog(_strModuleName, ex);
        //    }
        //} 
        #endregion
    }
}