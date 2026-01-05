using Application.Exceptions.Base;

namespace Application.Exceptions.Empresas
{
    public class EmpresaCnpjConflictException(string cnpj) : ConflictException($"já existe uma empresa registrada com o cnpj: {cnpj}")
    {
    }
}
