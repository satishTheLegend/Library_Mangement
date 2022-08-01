using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library_Mangement.Helper;
using Library_Mangement.Resx;
using Library_Mangement.Services;
using Library_Mangement.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BarcodeControl : StackLayout
    {
        #region Properties
        private readonly string _strModuleName = nameof(BarcodeControl);

        public static BindableProperty ParentBindingContextProperty =
        BindableProperty.Create(nameof(ParentBindingContext), typeof(object),
        typeof(BarcodeControl));

        /// <summary>
        /// Gets or sets the parent bindingcontext.
        /// </summary>
        public object ParentBindingContext
        {
            get => GetValue(ParentBindingContextProperty);
            set => SetValue(ParentBindingContextProperty, value);
        }
        #endregion

        #region Constructor
        public BarcodeControl()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Event Handlers
        private async void BarcodeScanner_Clicked(object sender, EventArgs e)
        {
            try
            {
                //bool camerastatus = await Common.GetCameraPermission();
                //if (camerastatus)
                //{
                //    ZXingScannerPage scanPage;
                //    scanPage = new ZXingScannerPage { DefaultOverlayShowFlashButton = true };
                //    scanPage.DefaultOverlayShowFlashButton = true;
                    
                //    scanPage.OnScanResult += (result) =>
                //    {
                //        scanPage.IsScanning = false;
                //        Device.BeginInvokeOnMainThread(async () =>
                //        {
                //            await Navigation.PopModalAsync();
                //            BarCodeEntry.Text = result.ToString();
                //        });
                //    };
                //    await Navigation.PushModalAsync(scanPage);
                //}
                //else
                //{
                //    string msg = string.Format("Please Camera Allow Permissions");
                //    await App.Current.MainPage.DisplayAlert("", msg, AppResources.Ok);
                //}
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}