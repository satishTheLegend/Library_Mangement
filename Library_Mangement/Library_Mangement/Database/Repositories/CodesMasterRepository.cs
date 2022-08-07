using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class CodesMasterRepository : IRepositories<tblCodesMaster>
    {
        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor
        public CodesMasterRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                _conn.CreateTableAsync<tblCodesMaster>().Wait();
            }
            catch (Exception ex)
            {
            }
        }

        #endregion

        #region Public Methods
        public async Task<List<tblCodesMaster>> GetListByGroupName(string groupName)
        {
            var list = await _conn.Table<tblCodesMaster>().ToListAsync();
            return await _conn.Table<tblCodesMaster>().Where(x=>x.GroupName == groupName).OrderBy(x=> x.CodeSeq).ToListAsync();
        }
        #endregion

        #region Implemented Methods
        public async Task<int> DeleteAllRecords()
        {
            return await _conn.ExecuteAsync("Delete from tblCodesMaster");
        }

        public Task<int> DeleteAsync(tblCodesMaster entity)
        {
            return _conn.DeleteAsync(entity);
        }

        public Task<tblCodesMaster> FindByIdAsync(int Id)
        {
            return _conn.Table<tblCodesMaster>().FirstOrDefaultAsync(x => x.Id == Id);
        }

        public Task<List<tblCodesMaster>> GetDataAsync()
        {
            return _conn.Table<tblCodesMaster>().ToListAsync();
        }

        public async Task<int> InsertAsync(tblCodesMaster entity)
        {
            int result = -1;
            tblCodesMaster book = await _conn.Table<tblCodesMaster>().FirstOrDefaultAsync(x => x.CodeText == entity.CodeText && x.CodeValue == entity.CodeValue);
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

        public Task<int> UpdateAsync(tblCodesMaster entity)
        {
            return _conn.UpdateAsync(entity);
        }

        #endregion
    }
}
