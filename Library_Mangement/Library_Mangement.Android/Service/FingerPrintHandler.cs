using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Hardware.Fingerprint;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace Library_Mangement.Droid.Service
{
    public class FingerPrintHandler : FingerprintManagerCompat.AuthenticationCallback
    {
        private Context mainActivity;

        public bool AuthResult;
        public FingerPrintHandler(Context mainActivity)
        {
            this.mainActivity = mainActivity;
        }
        internal void Startprog(FingerprintManagerCompat fingerprintManager, FingerprintManagerCompat.CryptoObject cryptoObject)
        {
            CancellationSignal cancellationSignal = new CancellationSignal();
        }
        public override void OnAuthenticationFailed()
        {
            Toast.MakeText(mainActivity, "Fingerprint Authentication failed!", ToastLength.Long).Show();
            MessagingCenter.Send("Auth", "Fail");
        }
        public override void OnAuthenticationSucceeded(FingerprintManagerCompat.AuthenticationResult result)
        {
            Toast.MakeText(mainActivity, "Fingerprint Authentication Success", ToastLength.Long).Show();
            MessagingCenter.Send("Auth", "Success");
        }
    }
}