using Application.Exceptions.Base;

namespace Application.Exceptions.Tenants
{
    public class TenantNotFoundException(long id) : NotFoundException($"Tenant Com o ID: {id} não encontrado.")
    {
    }
}
