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
    public partial class EntryControl : StackLayout
    {
        #region Properties
        private readonly string strModuleName = nameof(EntryControl); 
        public static BindableProperty ParentBindingContextProperty =
        BindableProperty.Create(nameof(ParentBindingContext), typeof(object),
        typeof(EntryControl));

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
        public EntryControl()
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