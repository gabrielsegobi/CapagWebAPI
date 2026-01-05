using Application.Exceptions.Base;

namespace Application.Exceptions.RegimesTributarios
{
    public class RegimeTributarioNotFoundException : NotFoundException
    {
        public RegimeTributarioNotFoundException(long id) : base($"Regime tributário com o ID {id} não foi encontrado")
        {
        }
    }
}
