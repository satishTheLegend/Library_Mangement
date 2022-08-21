using Android.OS;
using Library_Mangement.Droid.PlatformServices;
using Library_Mangement.Services.PlatformServices;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace Library_Mangement.Droid.PlatformServices
{
    public class FileHelper : IFileHelper
    {
        public string GetPublicFolderPath()
        {
            return Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads)?.AbsolutePath;
        }
    }
}