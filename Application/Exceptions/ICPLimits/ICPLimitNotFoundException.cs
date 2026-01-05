using Application.Exceptions.Base;

namespace Application.Exceptions.ICPLimits
{
    public class ICPLimitNotFoundException : NotFoundException
    {
        public ICPLimitNotFoundException(long Id) : base($"ICP Com ID: {Id} não encontrado")
        {
        }
    }
}
