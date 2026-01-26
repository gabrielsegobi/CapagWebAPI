using Application.Exceptions.Base;

namespace Application.Exceptions.ValorCalcVariaveis
{
    public class ValorCalcVariavelNotFoundException : NotFoundException
    {
        public ValorCalcVariavelNotFoundException(long id) : base($"Variável não encontrada para o ID: {id} ")
        {
        }
    }
}
