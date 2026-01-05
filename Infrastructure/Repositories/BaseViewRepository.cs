using Domain.Contracts.Views;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class BaseViewRepository<T> : IBaseViewRepository<T> where T : class
    {
        private readonly CPGDbContext _context;
        private readonly DbSet<T> _dbSet;

        public BaseViewRepository(CPGDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await _dbSet.AsNoTracking()
                               .FirstOrDefaultAsync(e => EF.Property<long>(e, "Id") == id);
        }

        public IQueryable<T> Query()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }
    }
}
