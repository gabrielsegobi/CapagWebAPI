using Application.Exceptions.Base;

namespace Application.Exceptions.RegDarf
{
    public class RegDarfNotFoundException : NotFoundException
    {
        public RegDarfNotFoundException(long id) : base($"Darf com o ID: {id} não encotrado.")
        {
        }
    }
}
