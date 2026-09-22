using Application.Exceptions.Base;

namespace Application.Exceptions.Capag
{
    public class ContaGreManualNotFoundException : NotFoundException
    {
        public ContaGreManualNotFoundException(long id)
            : base($"Conta manual GRE com o ID:{id} não foi encontrada.")
        {
        }
    }
}
