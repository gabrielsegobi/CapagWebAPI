using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Infrastructure.Interface
{
    public interface IBaseRepository<T> where T : class
    {
        #region Create
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        Task<long> AddAsyncAndGetId(T entity);
        #endregion

        #region Read
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(long id);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null, bool asNoTracking = true);
        #endregion

        #region Update
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        #endregion
         
        #region Delete
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
        #endregion

        #region Save
        Task<int> SaveChangesAsync();
        #endregion
    }
}
