using Application.Exceptions.Base;

namespace Application.Exceptions.RegsDctf
{
    public class RegDctfFileNotFoundException : NotFoundException
    {
        public RegDctfFileNotFoundException(long id) : base($"Não foi encontrado registros Dcft para o IdFilename: {id}")
        {
        }
    }
}
