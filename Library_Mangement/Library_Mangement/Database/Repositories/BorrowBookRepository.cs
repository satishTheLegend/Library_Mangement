using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class BorrowBookRepository : IRepositories<tblBorrowBook>
    {

        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor
        public BorrowBookRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                _conn.CreateTableAsync<tblBorrowBook>().Wait();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Public Methods
        public async Task<List<tblBorrowBook>> GetMyBooks(string userID)
        {
            return await _conn.Table<tblBorrowBook>().Where(x => x.UserID == userID && !x.IsExpired).ToListAsync();
        }
        public async Task<bool> UpdateBookStatus(string userID, string bookISBN)
        {
            bool isReturned = false;
            var borrowDetails = await _conn.Table<tblBorrowBook>().FirstOrDefaultAsync(x => x.UserID == userID && x.BookISBN == bookISBN && !x.IsExpired);
            if(borrowDetails != null)
            {
                borrowDetails.IsExpired = true;
                borrowDetails.EndBorrowDate = DateTime.Now;
                isReturned = Convert.ToBoolean(await UpdateAsync(borrowDetails));
            }
            return isReturned;
        }

        #endregion

        #region Implemented Methods
        public async Task<int> DeleteAllRecords()
        {
            return await _conn.ExecuteAsync("Delete from tblBorrowBook");
        }

        public async Task<int> DeleteAsync(tblBorrowBook entity)
        {
            return await _conn.DeleteAsync(entity);
        }

        public async Task<tblBorrowBook> FindByIdAsync(int Id)
        {
            return await _conn.Table<tblBorrowBook>().FirstOrDefaultAsync(x => x.ID == Id);
        }

        public async Task<List<tblBorrowBook>> GetDataAsync()
        {
            return await _conn.Table<tblBorrowBook>().ToListAsync();
        }

        public async Task<int> InsertAsync(tblBorrowBook entity)
        {
            int result = -1;
            tblBorrowBook borrowBook = await _conn.Table<tblBorrowBook>().FirstOrDefaultAsync(x => x.BorrowID == entity.BorrowID && x.UserID == entity.UserID && x.BookISBN == entity.BookISBN);
            if (borrowBook == null)
            {
                result = await _conn.InsertAsync(entity);
            }
            else
            {
                result = await UpdateAsync(entity);
            }
            return result;
        }

        public async Task<int> UpdateAsync(tblBorrowBook entity)
        {
            return await _conn.UpdateAsync(entity);
        }

       
        #endregion
    }
}
