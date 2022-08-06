using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using Library_Mangement.Helper;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class LogRepository : IRepositories<tblLogs>
    {
        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor 
        public LogRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                TimeSpan ts = new TimeSpan(0, 0, 0, 1);
                _conn.SetBusyTimeoutAsync(ts).Wait();   //Set busy timeout!!!
                _conn.CreateTableAsync<tblLogs>().Wait();

            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Public Methods

        public void AddLogs(string logType, string moduleName, string description)
        {
            Task.Run(async() => await AddDataLogs(logType, moduleName, description));
        }

        public async Task<int> AddDataLogs(string logType, string moduleName,string description)
        {
            tblLogs tblLogs = new tblLogs()
            {
                LogType = string.IsNullOrEmpty(logType) ? AppConfig.LogType_Info : logType,
                Datetime = DateTime.Now,
                Description = string.IsNullOrEmpty(description) ? "No Description Available" : description,
                ModuleName = string.IsNullOrEmpty(moduleName) ? string.Empty : moduleName,
                UserInfo = App.CurrentLoggedInUser == null ? string.Empty : $"{App.CurrentLoggedInUser.FirstName} {App.CurrentLoggedInUser.LastName} | {App.CurrentLoggedInUser.Email} | {App.CurrentLoggedInUser.UserName}",
            };
            return await InsertAsync(tblLogs);
        }
        #endregion

        #region Implemented Methods
        public async Task<int> DeleteAllRecords()
        {
            return await _conn.ExecuteAsync("Delete from tblLogs");
        }

        public async Task<int> DeleteAsync(tblLogs entity)
        {
            return await _conn.DeleteAsync(entity);
        }

        public async Task<tblLogs> FindByIdAsync(int Id)
        {
            return await _conn.Table<tblLogs>().FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<tblLogs>> GetDataAsync()
        {
            return await _conn.Table<tblLogs>().ToListAsync();
        }

        public async Task<int> InsertAsync(tblLogs entity)
        {
            return await _conn.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync(tblLogs entity)
        {
            return await _conn.UpdateAsync(entity);
        }
        #endregion
    }
}
