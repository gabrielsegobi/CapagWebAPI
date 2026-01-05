using Application.Exceptions.Base;

namespace Application.Exceptions.UsuariosTenant
{
    public class UsuarioTenantNotFoundException(long Id) : NotFoundException($"Usuário tenant com o ID: {Id} não encontado")
    {
    }
}
