using Domain.Entities.Sped.Ecf;
namespace Infrastructure.Interface
{
    public interface IBulkInsertService
    {
        Task FlushAsync(
            Dictionary<Type, IList<EcfBase>> buffer,
            CancellationToken cancellationToken = default
        );
    }
}
