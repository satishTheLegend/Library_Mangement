using Library_Mangement.Database.Interface;
using Library_Mangement.Database.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Repositories
{
    public class AddToCartRepository : IRepositories<tblAddToCart>
    {
        #region Properties
        private readonly SQLiteAsyncConnection _conn;
        #endregion

        #region Constructor
        public AddToCartRepository(string dbPath)
        {
            try
            {
                _conn = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.FullMutex, true);
                _conn.CreateTableAsync<tblAddToCart>().Wait();
            }
            catch (Exception ex)
            {
            }
        }
        #endregion

        #region Public Methods
        public async Task<List<tblAddToCart>> GetCartBooks(string userID, string type)
        {
            return await _conn.Table<tblAddToCart>().Where(x => x.UserID == userID && x.Type == type && !x.IsRemovedFCart).ToListAsync();
        }
        public async Task<List<tblAddToCart>> GetWishListBooks(string userID, string type)
        {
            return await _conn.Table<tblAddToCart>().Where(x => x.UserID == userID && x.Type == type && !x.IsRemovedFWL).ToListAsync();
        }
        public async Task<bool> RemoveBookFromCart(string userID, string bookActionId)
        {
            bool isBookRemoved = false;
            var bookData = await _conn.Table<tblAddToCart>().FirstOrDefaultAsync(x => x.UserID == userID && x.CartID == bookActionId && !x.IsRemovedFCart);
            if(bookData != null)
            {
                bookData.RemoveCartDate = DateTime.Now;
                bookData.IsRemovedFCart = true;
                isBookRemoved = Convert.ToBoolean(await UpdateAsync(bookData));
            }
            return isBookRemoved;
        }
        #endregion

        #region Implemented Methods
        public async Task<int> DeleteAllRecords()
        {
            return await _conn.ExecuteAsync("Delete from tblAddToCart");
        }

        public async Task<int> DeleteAsync(tblAddToCart entity)
        {
            return await _conn.DeleteAsync(entity);
        }

        public async Task<tblAddToCart> FindByIdAsync(int Id)
        {
            return await _conn.Table<tblAddToCart>().FirstOrDefaultAsync(x => x.ID == Id);
        }

        public async Task<List<tblAddToCart>> GetDataAsync()
        {
            return await _conn.Table<tblAddToCart>().ToListAsync();
        }

        public async Task<int> InsertAsync(tblAddToCart entity)
        {
            int result = -1;
            tblAddToCart borrowBook = await _conn.Table<tblAddToCart>().FirstOrDefaultAsync(x => x.CartID == entity.CartID && x.UserID == entity.UserID 
            && x.BookISBN == entity.BookISBN);
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

        public async Task<int> UpdateAsync(tblAddToCart entity)
        {
            return await _conn.UpdateAsync(entity);
        }
        #endregion
    }
}
