namespace Domain.Interfaces
{
    public interface IQueryProviderExecutor<TEntity>
    {
        Task<int> CountAsync();
        Task<List<TEntity>> ToListAsync(int skip, int take);
    }
}
