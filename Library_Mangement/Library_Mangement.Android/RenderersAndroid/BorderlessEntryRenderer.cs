using Android.Content;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Library_Mangement.Droid.RenderersAndroid;
using Library_Mangement.Renderers;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessBorderlessEntryRenderer))]
namespace Library_Mangement.Droid.RenderersAndroid
{
    public class BorderlessBorderlessEntryRenderer : EntryRenderer
    {
        public BorderlessBorderlessEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement == null)
            {
                Control.Background = null;
            }
        }
    }
}