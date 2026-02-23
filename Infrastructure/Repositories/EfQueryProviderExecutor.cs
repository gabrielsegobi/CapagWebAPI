using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class EfQueryProviderExecutor<TEntity> : IQueryProviderExecutor<TEntity>
    {
        private readonly IQueryable<TEntity> _query;

        public EfQueryProviderExecutor(IQueryable<TEntity> query)
        {
            _query = query;
        }

        public Task<int> CountAsync() => _query.CountAsync();
        public Task<List<TEntity>> ToListAsync(int skip, int take)
            => _query.Skip(skip).Take(take).ToListAsync();
    }
}
