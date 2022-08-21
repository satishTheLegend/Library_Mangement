using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class SettingsRepository : IRepositories<tblSettings>
    {
        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor
        public SettingsRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                _conn.CreateTableAsync<tblSettings>().Wait();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Public Methods
        public async Task<bool> IsDataContainsInValue(string key ,string value)
        {
            var data = await _conn.Table<tblSettings>().FirstOrDefaultAsync(x => x.Key == key && x.Value.Contains(value));
            if(data != null)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        #endregion

        #region Implemented Methods
        public async Task<int> DeleteAllRecords()
        {
            return await _conn.DeleteAllAsync<tblSettings>();
        }

        public async Task<int> DeleteAsync(tblSettings entity)
        {
            return await _conn.DeleteAsync<tblSettings>(entity);
        }

        public async Task<tblSettings> FindByKeyAsync(string key)
        {
            return await _conn.Table<tblSettings>().FirstOrDefaultAsync(x=> x.Key == key);
        }

        public Task<tblSettings> FindByIdAsync(int Id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<tblSettings>> GetDataAsync()
        {
            return await _conn.Table<tblSettings>().ToListAsync();
        }

        public async Task<int> InsertAsync(tblSettings entity)
        {
            int result = 0;
            var data = await _conn.Table<tblSettings>().FirstOrDefaultAsync(x => x.Value == entity.Value);
            if(data != null)
            {
                result = await UpdateAsync(entity);
            }
            else
            {
                result = await _conn.InsertAsync(entity);
            }
            return result;
        }

        public async Task<int> UpdateAsync(tblSettings entity)
        {
            return await _conn.UpdateAsync(entity);
        }
        #endregion
    }
}
