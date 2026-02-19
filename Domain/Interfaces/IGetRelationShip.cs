using Domain.Contracts;

namespace Domain.Interfaces
{
    public interface IGetRelationShip
    {
        Task<List<Relationship>> ExecuteAsync(string type, CancellationToken cancellationToken);
    }
}
