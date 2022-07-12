using Library_Mangement.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Library_Mangement.Database.Interface
{
    public interface IRepositories<T> where T : BaseModel
    {
        Task<List<T>> GetDataAsync();
        Task<int> InsertAsync(T entity);
        Task<int> DeleteAsync(T entity);
        Task<int> UpdateAsync(T entity);
        Task<T> FindByIdAsync(int Id);
        Task<int> DeleteAllRecords();
    }
}
