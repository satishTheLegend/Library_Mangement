using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Library_Mangement.Views.Cards
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookUICard : StackLayout
    {
        #region Properties
        private readonly string _strModuleName = nameof(BookUICard);
        public static BindableProperty ParentBindingContextProperty =
         BindableProperty.Create(nameof(ParentBindingContext), typeof(object),
         typeof(BookUICard), null);

        public object ParentBindingContext
        {
            get { return GetValue(ParentBindingContextProperty); }
            set { SetValue(ParentBindingContextProperty, value); }
        }
        #endregion

        #region Constructor
        public BookUICard()
        {
            InitializeComponent();
        }
        #endregion

    }
}