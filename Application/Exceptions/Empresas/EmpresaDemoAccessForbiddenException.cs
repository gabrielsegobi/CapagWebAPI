using Application.Exceptions.Base;

namespace Application.Exceptions.Empresas
{
    public class EmpresaDemoAccessForbiddenException()
        : ForbiddenException("Usuário demo não possui acesso a esta empresa.")
    {
    }
}
