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
    public partial class DropDownListControl : StackLayout
    {
        #region Properties
        private readonly string _strModuleName = nameof(DropDownListControl);
        public static BindableProperty ParentBindingContextProperty =
        BindableProperty.Create(nameof(ParentBindingContext), typeof(object),
        typeof(DropDownListControl), null);

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
        public DropDownListControl()
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