using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
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

        #region Database Methods
        public Task<bool> DeleteAllRecords()
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(tblLogs entity)
        {
            throw new NotImplementedException();
        }

        public Task<tblLogs> FindByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<tblLogs>> GetDataAsync()
        {
            throw new NotImplementedException();
        }

        public Task<int> InsertAsync(tblLogs entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(tblLogs entity)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
