using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Com.Airbnb.Lottie;

namespace Library_Mangement.Droid
{
    [Activity(Label = "OCLM_System", Icon = "@mipmap/icon", Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashScreen : Activity, Android.Animation.Animator.IAnimatorListener
    {
        public void OnAnimationCancel(Animator animation)
        {

        }

        public void OnAnimationEnd(Animator animation)
        {
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        public void OnAnimationRepeat(Animator animation)
        {

        }

        public void OnAnimationStart(Animator animation)
        {

        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.SplashLayout);
            var animation = FindViewById<Com.Airbnb.Lottie.LottieAnimationView>(Resource.Id.animation_view);
            var animation2 = FindViewById<Com.Airbnb.Lottie.LottieAnimationView>(Resource.Id.animation_view1);
            animation.AddAnimatorListener(this);
            animation2.AddAnimatorListener(this);
        }
    }
}