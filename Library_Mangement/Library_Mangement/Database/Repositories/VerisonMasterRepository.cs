using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using Library_Mangement.Model.ApiResponse.GETModels;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class VerisonMasterRepository : IRepositories<tblVerisonMaster>
    {
        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor
        public VerisonMasterRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                _conn.CreateTableAsync<tblVerisonMaster>().Wait();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Public Methods
        public async Task<bool> IsMasterDataVersionMissing(MasterDataList masterItem)
        {
            tblVerisonMaster ver = await _conn.Table<tblVerisonMaster>().FirstOrDefaultAsync(x => x.Key == masterItem.key && x.Value == masterItem.value);
            if (ver == null)
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
            return await _conn.ExecuteAsync("Delete from tblVerisonMaster");
        }

        public Task<int> DeleteAsync(tblVerisonMaster entity)
        {
            return _conn.DeleteAsync(entity);
        }

        public Task<tblVerisonMaster> FindByIdAsync(int Id)
        {
            return _conn.Table<tblVerisonMaster>().FirstOrDefaultAsync(x => x.ID == Id);
        }

        public Task<List<tblVerisonMaster>> GetDataAsync()
        {
            return _conn.Table<tblVerisonMaster>().ToListAsync();
        }

        public async Task<int> InsertAsync(tblVerisonMaster entity)
        {
            int result = -1;
            tblVerisonMaster book = await _conn.Table<tblVerisonMaster>().FirstOrDefaultAsync(x => x.Key == entity.Key);
            if (book == null)
            {
                result = await _conn.InsertAsync(entity);
            }
            else
            {
                result = await UpdateAsync(entity);
            }
            return result;
        }

        public Task<int> UpdateAsync(tblVerisonMaster entity)
        {
            return _conn.UpdateAsync(entity);

            #endregion
        }
    }
}
