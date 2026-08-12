using Application.Exceptions.Base;

namespace Application.Exceptions.CodigosRegistroDescricao
{
    public class CodigoRegistroDescricaoNotFoundException : NotFoundException
    {
        public CodigoRegistroDescricaoNotFoundException(long id)
            : base($"Código de registro descrição não encontrado para o ID: {id}")
        {
        }
    }
}
