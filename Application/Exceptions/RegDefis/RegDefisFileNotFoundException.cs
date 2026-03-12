using Application.Exceptions.Base;

namespace Application.Exceptions.RegDefis
{
    public class RegDefisFileNotFoundException : NotFoundException
    {
        public RegDefisFileNotFoundException(long id) : base($"Não foi eocntrado nenhum registro defis para esse IdFilename: {id}")
        {
        }
    }
}
