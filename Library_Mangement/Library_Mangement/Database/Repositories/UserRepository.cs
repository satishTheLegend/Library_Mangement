using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class UserRepository : IRepositories<tblUser>
    {
        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor
        public UserRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                _conn.CreateTableAsync<tblUser>().Wait();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Public Methods
        public tblUser GetActiveUserData()
        {
            tblUser result = null;
            try
            {
                result = _conn.Table<tblUser>().FirstOrDefaultAsync().Result;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public async Task<int> AddCatagories(List<string> selectedItems, string userToken)
        {
            int res = 0;
            var result = _conn.Table<tblUser>().FirstOrDefaultAsync(x=> x.UserToken == userToken).Result;
            if(result != null)
            {
                result.Catagories = string.Join(",", selectedItems);
                res = await UpdateAsync(result);
            }
            return res;
        }

        #endregion

        #region Implemented Methods
        public async Task<int> DeleteAllRecords()
        {
            return await _conn.ExecuteAsync("Delete from tblUser");
        }

        public Task<int> DeleteAsync(tblUser entity)
        {
            return _conn.DeleteAsync(entity);
        }

        public Task<tblUser> FindByIdAsync(int Id)
        {
            return _conn.Table<tblUser>().FirstOrDefaultAsync(x => x.ID == Id);
        }

        public Task<List<tblUser>> GetDataAsync()
        {
            return _conn.Table<tblUser>().ToListAsync();
        }

        public Task<int> InsertAsync(tblUser entity)
        {
            return _conn.InsertAsync(entity);
        }

        public Task<int> UpdateAsync(tblUser entity)
        {
            return _conn.UpdateAsync(entity);
        }
        #endregion
    }
}
