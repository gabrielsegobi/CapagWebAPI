using Application.Exceptions.Base;

namespace Application.Exceptions.Operations
{
    public class OperationWithErrorNotFoundException : NotFoundException
    {
        public OperationWithErrorNotFoundException(long id) : base($"Não foi encontrado nenhum arquivo com erro para ser reprocessado na empresa com o ID: {id}")
        {
        }
    }
}
