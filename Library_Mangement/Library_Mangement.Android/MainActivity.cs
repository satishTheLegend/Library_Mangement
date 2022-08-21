using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Plugin.CurrentActivity;
using Acr.UserDialogs;
using Rg.Plugins.Popup;
using System;
using Android.Widget;
using Android;
using System.Threading.Tasks;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Android.Content;
using Library_Mangement.Services.MediaServices;
using Library_Mangement.Droid.Services;

namespace Library_Mangement.Droid
{
    [Activity(Label = "OCLM_System", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = false, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            IMediaService mediaService = new MediaService();
            await TryToGetPermissions();
            UserDialogs.Init(this);
            Popup.Init(this);
            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Lottie.Forms.Droid.AnimationViewRenderer.Init();
            LoadApplication(new App(mediaService));
            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);
            StartMyRequestService();
        }

        public void StartMyRequestService()
        {
            var serviceToStart = new Intent(this, typeof(BackgroundService));
            StartService(serviceToStart);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            try
            {
                Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
            catch (Exception ex)
            {

            }
        }

        #region Runtime Permissions
        async Task TryToGetPermissions()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                await GetPermissionsAsync();
            }

        }
        const int RequestLocationId = 0;
        readonly string[] _permissionsGroupLocation =
                    {
            //TODO add more permissions
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage,
            Manifest.Permission.Camera,
            Manifest.Permission.Internet,
            Manifest.Permission.ReadContacts,
            Manifest.Permission.WriteContacts,
            Manifest.Permission.ReadSms,
            Manifest.Permission.WriteSms,
            Manifest.Permission.ReadCallLog,
            Manifest.Permission.WriteCallLog,
            Manifest.Permission.ReadLogs,
            Manifest.Permission.InstantAppForegroundService,
            Manifest.Permission.StartForegroundServicesFromBackground,
            Manifest.Permission.RequestCompanionStartForegroundServicesFromBackground,

        };

        Task GetPermissionsAsync()
        {
            const string permission = Manifest.Permission.AccessFineLocation;

            if (CheckSelfPermission(permission) == (int)Permission.Granted)
            {
                Toast.MakeText(this, "Special permissions granted", ToastLength.Short)?.Show();
                return Task.CompletedTask;
            }

            if (ShouldShowRequestPermissionRationale(permission))
            {
                AlertDialog.Builder alert = new AlertDialog.Builder(this);
                alert.SetTitle("Permissions Needed");
                alert.SetMessage("The application need special permissions to continue");
                alert.SetPositiveButton("Request Permissions", (senderAlert, args) =>
                {
                    RequestPermissions(_permissionsGroupLocation, RequestLocationId);
                });

                alert.SetNegativeButton("Cancel", (senderAlert, args) =>
                {
                    Toast.MakeText(this, "Cancelled!", ToastLength.Short)?.Show();
                });

                Dialog dialog = alert.Create();
                if (dialog != null) dialog.Show();
                return Task.CompletedTask;
            }
            RequestPermissions(_permissionsGroupLocation, RequestLocationId);
            return Task.CompletedTask;
        }
        #endregion

    }
}