using Library_Mangement.Services;
using Library_Mangement.ViewModels;
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
    public partial class RadioButtonControl : StackLayout
    {
        #region Properties
        private readonly string _strModuleName = nameof(RadioButtonControl);
        public static BindableProperty ParentBindingContextProperty =
        BindableProperty.Create(nameof(ParentBindingContext), typeof(object),
        typeof(RadioButtonControl));

        public object ParentBindingContext
        {
            get => GetValue(ParentBindingContextProperty);
            set => SetValue(ParentBindingContextProperty, value);
        }

        #endregion

        #region Constructor
        public RadioButtonControl()
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

        #region Event Handler
        private void Rb_Checked(object sender, EventArgs e)
        {
            try
            {
                if (sender is RadioButton radioButton)
                {
                    if (ParentBindingContext is DynamicPropertyDataViewModel fieldData)
                    {
                        fieldData.FieldValue = radioButton.Content.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        #endregion


    }
}