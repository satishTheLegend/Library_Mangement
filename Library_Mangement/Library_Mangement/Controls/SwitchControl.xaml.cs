using Library_Mangement.Services;
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
    public partial class SwitchControl : StackLayout
    {
        #region Properties
        private readonly string _strModuleName = nameof(SwitchControl);
        public static BindableProperty ParentBindingContextProperty =
        BindableProperty.Create(nameof(ParentBindingContext), typeof(object),
        typeof(SwitchControl), null);

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
        public SwitchControl()
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

    }
}