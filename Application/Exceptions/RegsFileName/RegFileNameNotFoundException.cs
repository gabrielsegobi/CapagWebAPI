using Application.Exceptions.Base;

namespace Application.Exceptions.RegsFileName
{
    public class RegFileNameNotFoundException : NotFoundException
    {
        public RegFileNameNotFoundException(long id) : base($"Não foi encontrado o arquivo com ID: {id}")
        {
        }
    }
}
