using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Library_Mangement.Views.FlyoutView
{
    public class DashboardFlyoutViewFlyoutMenuItem
    {
        public DashboardFlyoutViewFlyoutMenuItem()
        {
            TargetType = typeof(DashboardFlyoutViewFlyoutMenuItem);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public ImageSource Icon { get; set; }

        public Type TargetType { get; set; }
    }
}