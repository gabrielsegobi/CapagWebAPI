using Infrastructure.Context;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        protected readonly CPGDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseRepository(CPGDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<T>();
        }

        #region Create
        public virtual async Task AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await _dbSet.AddAsync(entity);
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            await _dbSet.AddRangeAsync(entities);
        }
        #endregion

        #region Read
        public virtual async Task<T?> GetByIdAsync(long id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public virtual IQueryable<T> Query(Expression<Func<T, bool>>? predicate = null, bool asNoTracking = true)
        {
            IQueryable<T> query = _dbSet;
            if (asNoTracking) query = query.AsNoTracking();
            if (predicate != null) query = query.Where(predicate);
            return query;
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _context.Set<T>().CountAsync();

            return await _context.Set<T>().CountAsync(predicate);
        }

        #endregion

        #region Update
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.UpdateRange(entities);
        }
        #endregion

        #region Delete
        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            _dbSet.Remove(entity);
        }

        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            _dbSet.RemoveRange(entities);
        }
        #endregion

        #region Save
        public virtual async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<long> AddAsyncAndGetId(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();

            var idProperty = entity.GetType().GetProperty("Id");

            if (idProperty == null)
                throw new InvalidOperationException($"A entidade {typeof(T).Name} não possui a propriedade 'Id'.");

            return (long)idProperty.GetValue(entity)!;
        }
        #endregion
    }
}
