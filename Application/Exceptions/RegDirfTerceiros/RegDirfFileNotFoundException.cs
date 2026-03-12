using Application.Exceptions.Base;

namespace Application.Exceptions.RegDirfTerceiros
{
    public class RegDirfFileNotFoundException : NotFoundException
    {
        public RegDirfFileNotFoundException(long id) : base($"Não foi encontrado registros dirfs para esse IdFilename: {id}")
        {
        }
    }
}
