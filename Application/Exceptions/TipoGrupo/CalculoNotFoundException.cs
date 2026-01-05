using Application.Exceptions.Base;

namespace Application.Exceptions.TipoGrupo
{
    public class CalculoNotFoundException : NotFoundException
    {
        public CalculoNotFoundException(string tag) : base($"Cálculo não encontrado para a Tag: {tag}")
        {
        }
    }
}
