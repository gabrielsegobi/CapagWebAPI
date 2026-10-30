using Domain.Entities;

namespace Infrastructure.Interface
{
    public interface IDemoEmpresaAccessService
    {
        bool IsDemoUser { get; }

        IQueryable<Empresa> FilterEmpresas(IQueryable<Empresa> query);

        bool CanAccessCnpj(string? cnpj);

        Task EnsureCanAccessEmpresaAsync(long idEmpresa, CancellationToken cancellationToken = default);
    }
}
