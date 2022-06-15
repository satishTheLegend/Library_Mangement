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
        #endregion

        #region Constructor
        public Loader()
        {
            InitializeComponent();
        }
        #endregion

        #region Event Handlers
        private static void LoaderTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = (Loader)bindable;
            control.AnimatedLoaderText.Text = (string)newValue;
        }
        #endregion

    }
}