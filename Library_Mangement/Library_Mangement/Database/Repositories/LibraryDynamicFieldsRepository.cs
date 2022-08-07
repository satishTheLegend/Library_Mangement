using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class LibraryDynamicFieldsRepository : IRepositories<tblLibraryDynamicFields>
    {
        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor
        public LibraryDynamicFieldsRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                _conn.CreateTableAsync<tblLibraryDynamicFields>().Wait();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Public Methods
        public async Task<List<tblLibraryDynamicFields>> GetFieldsByPageName(string pageName)
        {
            return await _conn.Table<tblLibraryDynamicFields>().Where(x => x.PageName == pageName).ToListAsync();
        }
        #endregion

        #region Implemented Methods
        public async Task<int> DeleteAllRecords()
        {
            return await _conn.ExecuteAsync("Delete from tblLibraryDynamicFields");
        }

        public async Task<int> DeleteAsync(tblLibraryDynamicFields entity)
        {
            return await _conn.DeleteAsync(entity);
        }

        public async Task<tblLibraryDynamicFields> FindByIdAsync(int Id)
        {
            return await _conn.Table<tblLibraryDynamicFields>().FirstOrDefaultAsync(x => x.FieldId == Id);
        }

        public async Task<List<tblLibraryDynamicFields>> GetDataAsync()
        {
            return await _conn.Table<tblLibraryDynamicFields>().ToListAsync();
        }

        public async Task<int> InsertAsync(tblLibraryDynamicFields entity)
        {
            int result = -1;
            tblLibraryDynamicFields book = await _conn.Table<tblLibraryDynamicFields>().FirstOrDefaultAsync(x => x.FieldId == entity.FieldId && x.FieldName == x.FieldName);
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

        public async Task<int> UpdateAsync(tblLibraryDynamicFields entity)
        {
            return await _conn.UpdateAsync(entity);
        } 
        #endregion
    }
}
