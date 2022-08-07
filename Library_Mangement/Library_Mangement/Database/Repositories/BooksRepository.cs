using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class BooksRepository : IRepositories<tblBook>
    {
        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor
        public BooksRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                _conn.CreateTableAsync<tblBook>().Wait();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Public Methods

        #endregion

        #region Implemented Methods
        public async Task<int> DeleteAllRecords()
        {
            return await _conn.ExecuteAsync("Delete from tblBook");
        }

        public Task<int> DeleteAsync(tblBook entity)
        {
            return _conn.DeleteAsync(entity);
        }

        public Task<tblBook> FindByIdAsync(int Id)
        {
            return _conn.Table<tblBook>().FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<tblBook>> GetDataAsync()
        {
            return await _conn.Table<tblBook>().ToListAsync();
        }

        public async Task<int> InsertAsync(tblBook entity)
        {
            int result = -1;
            tblBook book = await _conn.Table<tblBook>().FirstOrDefaultAsync(x => x.Title == entity.Title && x.PngLink == entity.PngLink && x.PngFilePath == entity.PngFilePath && x.ISBN == entity.ISBN && x.Title == entity.Title && x.Authors == entity.Authors);
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

        public Task<int> UpdateAsync(tblBook entity)
        {
            return _conn.UpdateAsync(entity);
        }

        public async Task<tblBook> GetBookByISBNId(string iSBN)
        {
            return await _conn.Table<tblBook>().FirstOrDefaultAsync(x => x.ISBN == iSBN);
        }
        #endregion
    }
}
