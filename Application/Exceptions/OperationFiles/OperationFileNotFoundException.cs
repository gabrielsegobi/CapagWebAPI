using Application.Exceptions.Base;

namespace Application.Exceptions.OperationFiles
{
    public class OperationFileNotFoundException : NotFoundException
    {
        public OperationFileNotFoundException(long Id) : base($"Arquivo não encontrado para o ID: {Id}.")
        {
        }
    }
}
