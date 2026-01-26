using Application.Exceptions.Base;

namespace Application.Exceptions.ICPAnterior
{
    public class ICPAnteriorNotFoundException : NotFoundException
    {
        public ICPAnteriorNotFoundException(long id) : base($"ICP não encontrado para o ID: {id}")
        {
        }
    }
}
