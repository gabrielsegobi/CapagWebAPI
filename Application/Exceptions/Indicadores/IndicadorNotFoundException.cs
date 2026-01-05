using Application.Exceptions.Base;

namespace Application.Exceptions.Indicadores
{
    public class IndicadorNotFoundException : NotFoundException
    {
        public IndicadorNotFoundException(long id) : base($"Indicador com o ID{id} não foi encontrado")
        {
        }
    }
}
