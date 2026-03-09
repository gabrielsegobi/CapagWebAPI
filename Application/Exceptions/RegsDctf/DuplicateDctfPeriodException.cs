using Application.Exceptions.Base;

namespace Application.Exceptions.RegsDctf
{
    public class DuplicateDctfPeriodException : ConflictException
    {
        public DuplicateDctfPeriodException(string periodo) : base($"Já existe uma DCTF para o período: {periodo}.")
        {
        }
    }
}
