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
    public partial class CheckBoxListControl : StackLayout
    {
        #region Properties
        private readonly string _strModuleName = nameof(CheckBoxListControl);
        public static BindableProperty ParentBindingContextProperty =
             BindableProperty.Create(nameof(ParentBindingContext), typeof(object),
             typeof(CheckBoxListControl), null);

        public object ParentBindingContext
        {
            get => GetValue(ParentBindingContextProperty);
            set => SetValue(ParentBindingContextProperty, value);
        }
        #endregion

        #region Constructor
        public CheckBoxListControl()
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
        private void CbControl_CheckChanged(object sender, EventArgs e)
        {
            try
            {
                var InputKitCheckBox = sender as Plugin.InputKit.Shared.Controls.CheckBox;
                var context = ParentBindingContext as DynamicPropertyDataViewModel;
                if (InputKitCheckBox != null && context != null)
                {
                    if (string.IsNullOrEmpty(context.FieldValue))
                    {
                        context.FieldValue = InputKitCheckBox.Text;
                    }
                    else
                    {
                        context.FieldValue = $",{InputKitCheckBox.Text}";
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