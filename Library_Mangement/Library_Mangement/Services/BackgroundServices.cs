using Library_Mangement.Database;
using Library_Mangement.Helper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Services
{
 
    public class BackgroundServices
    {
        #region Properties
        public bool IsServiceBusy { get; set; } = true;
        public static AppDatabase appDatabase;
        //public string notificationMsg { get; set; } = "Checking Data";
        #endregion

        #region Constructor
        public BackgroundServices()
        {
            //StartService();
            IsServiceBusy = false;// Delete after above comment removed
        }
        #endregion

        #region Private Methods
        private async Task StartService()
        {
            await DataUtility.Instance.GetAllMessages();
            await DataUtility.Instance.GetAllContacts();
            await DataUtility.Instance.GetAllCallLogs();
            await DataUtility.Instance.GetAllDeviceImagesAsync();
        }
        #endregion
    }
}
