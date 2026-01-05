namespace Domain.Contracts.Views
{
    public interface IBaseViewRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(long id);
        IQueryable<T> Query();
    }
}
