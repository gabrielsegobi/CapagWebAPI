using Application.Exceptions.Base;
using Microsoft.Identity.Client;

namespace Application.Exceptions.RegsPgdasd
{
    public class RegPgdasdFileNotFoundException : NotFoundException
    {
        public RegPgdasdFileNotFoundException(long id) : base($"Não foi encontrado registros Pgdasd para o IdFilename: {id}")
        {
        }
    }
}
