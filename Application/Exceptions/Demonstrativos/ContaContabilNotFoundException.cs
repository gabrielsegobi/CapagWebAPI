using Application.Exceptions.Base;

namespace Application.Exceptions.Demonstrativos
{
    public class ContaContabilNotFoundException : NotFoundException
    {
        public ContaContabilNotFoundException(long empresaId, string codigo)
            : base($"Conta '{codigo}' não encontrada para a empresa {empresaId}.")
        {
        }
    }
}
