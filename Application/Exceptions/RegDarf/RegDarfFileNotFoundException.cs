using Application.Exceptions.Base;

namespace Application.Exceptions.RegDarf
{
    public class RegDarfFileNotFoundException : NotFoundException
    {
        public RegDarfFileNotFoundException(long id) : base($"Não foi encontrado nenhum registro darf com esse IdFilename:{id}.")
        {
        }
    }
}
