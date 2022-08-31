using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Loader : StackLayout
    {
        #region Properties
        public static BindableProperty ParentBindingContextProperty =
        BindableProperty.Create(nameof(ParentBindingContext), typeof(object),
        typeof(Loader), null);

        public object ParentBindingContext
        {
            get { return GetValue(ParentBindingContextProperty); }
            set { SetValue(ParentBindingContextProperty, value); }
        }

        public string AnimationAssetFileName
        {
            get => (string)GetValue(AnimationAssetFileNameProperty);
            set => SetValue(AnimationAssetFileNameProperty, value);
        }

        public static BindableProperty AnimationAssetFileNameProperty = BindableProperty.Create(
                                propertyName: "AnimationAssetFileName",
                                returnType: typeof(string),
                                declaringType: typeof(Loader),
                                defaultValue: "library_Loader.json",
                                defaultBindingMode: BindingMode.TwoWay,
                                propertyChanged: AnimationAssetFileNamePropertyChanged);

        public string LoaderText
        {
            get => (string)GetValue(LoaderTextProperty);
            set => SetValue(LoaderTextProperty, value);
        }

        public static BindableProperty LoaderTextProperty = BindableProperty.Create(
                                propertyName: "LoaderText",
                                returnType: typeof(string),
                                declaringType: typeof(Loader),
                                defaultValue: "",
                                defaultBindingMode: BindingMode.TwoWay,
                                propertyChanged: LoaderTextPropertyChanged);

        public double LoaderProgress
        {
            get => (double)GetValue(LoaderProgressProperty);
            set
            {
                SetValue(LoaderProgressProperty, value);
                Progress.IsVisible = true;
                if (value < 1)
                {
                    Progress.IsVisible = false;
                }
            }
        }

        public static BindableProperty LoaderProgressProperty = BindableProperty.Create(
                                propertyName: "LoaderProgress",
                                returnType: typeof(double),
                                declaringType: typeof(Loader),
                                defaultValue: 0.0,
                                defaultBindingMode: BindingMode.TwoWay,
                                propertyChanged: LoaderProgressPropertyChanged);

        public Color LoaderTextColor
        {
            get => (Color)GetValue(LoaderTextColorProperty);
            set => SetValue(LoaderTextColorProperty, value);
        }

        public static BindableProperty LoaderTextColorProperty = BindableProperty.Create(
                                propertyName: "LoaderTextColor",
                                returnType: typeof(Color),
                                declaringType: typeof(Loader),
                                defaultValue: Color.White,
                                defaultBindingMode: BindingMode.TwoWay,
                                propertyChanged: LoaderTextColorPropertyChanged);

        public bool LoaderIsVisible
        {
            get => (bool)GetValue(LoaderIsVisibleProperty);
            set => SetValue(LoaderIsVisibleProperty, value);
        }

        public static BindableProperty LoaderIsVisibleProperty = BindableProperty.Create(
                                propertyName: "LoaderIsVisible",
                                returnType: typeof(bool),
                                declaringType: typeof(Loader),
                                defaultValue: true,
                                defaultBindingMode: BindingMode.TwoWay,
                                propertyChanged: LoaderIsVisiblePropertyChanged);

        public double LoaderBackgroundOpacity
        {
            get => (double)GetValue(LoaderBackgroundOpacityProperty);
            set => SetValue(LoaderBackgroundOpacityProperty, value);
        }

        public static BindableProperty LoaderBackgroundOpacityProperty = BindableProperty.Create(
                                propertyName: "LoaderBackgroundOpacity",
                                returnType: typeof(double),
                                declaringType: typeof(Loader),
                                defaultValue: 0.8,
                                defaultBindingMode: BindingMode.TwoWay,
                                propertyChanged: LoaderBackgroundOpacityPropertyChanged);

        public Color LoaderBackgroundColor
        {
            get => (Color)GetValue(LoaderBackgroundColorProperty);
            set => SetValue(LoaderBackgroundColorProperty, value);
        }

        public static BindableProperty LoaderBackgroundColorProperty = BindableProperty.Create(
                                propertyName: "LoaderBackgroundColor",
                                returnType: typeof(Color),
                                declaringType: typeof(Loader),
                                defaultValue: Color.Gray,
                                defaultBindingMode: BindingMode.TwoWay,
                                propertyChanged: LoaderBackgroundColorPropertyChanged);
        public int LoaderHeight
        {
            get => (int)GetValue(LoaderHeightProperty);
            set => SetValue(LoaderHeightProperty, value);
        }

        public static BindableProperty LoaderHeightProperty = BindableProperty.Create(
                                propertyName: "LoaderHeight",
                                returnType: typeof(int),
                                declaringType: typeof(Loader),
                                defaultValue: 100,
                                defaultBindingMode: BindingMode.TwoWay,
                                propertyChanged: LoaderHeightPropertyChanged);

       

        #endregion

        #region Constructor
        public Loader()
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
        private static void LoaderTextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (Loader)bindable;
                control.AnimatedLoaderText.TextColor = (Color)newValue;
            }
            catch (Exception ex)
            {

            }
        }

        private static void LoaderHeightPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (Loader)bindable;
                control.AnimatedLoaderText.HeightRequest = (int)newValue;
            }
            catch (Exception ex)
            {

            }
        }
        private static void LoaderProgressPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Loader)bindable;
            if((double)newValue > 100)
            {
                control.Progress.Progress = 100;
                control.progressLabel.Text = $"100 %";
            }
            else
            {
                control.Progress.Progress = (double)newValue;
                if (control.Progress.Progress > 0)
                {
                    control.progressLabel.Text = $"{Convert.ToInt32(newValue)} %";
                }
            }
            
            
        }

        private static void AnimationAssetFileNamePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Loader)bindable;
            control.animationView.Animation = (string)newValue;
            switch ((string)newValue)
            {
                case "library_Loader.json":
                    break;
                case "Downloading_Files.json":
                    control.animationView.HeightRequest = (double)200;
                    control.animationView.WidthRequest = (double)200;
                    control.animationView.Margin = new Thickness(0, 0, 0, 50);
                    break;
                default:
                    break;
            }
        }

        private static void LoaderTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (Loader)bindable;
                control.AnimatedLoaderText.Text = (string)newValue;
            }
            catch (Exception ex)
            {

            }
        }

        private static void LoaderIsVisiblePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (Loader)bindable;
                control.LoaderStack.IsVisible = (bool)newValue;
            }
            catch (Exception ex)
            {

            }
        }

        private static void LoaderBackgroundOpacityPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (Loader)bindable;
                control.LoaderFrame.Opacity = (float)newValue;
            }
            catch (Exception ex)
            {

            }
        }
        private static void LoaderBackgroundColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            try
            {
                var control = (Loader)bindable;
                control.LoaderFrame.BackgroundColor = (Color)newValue;
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

    }
}