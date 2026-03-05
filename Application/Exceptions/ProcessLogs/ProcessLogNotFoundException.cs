using Application.Exceptions.Base;

namespace Application.Exceptions.ProcessLogs
{
    public class ProcessLogNotFoundException : NotFoundException
    {
        public ProcessLogNotFoundException(long Id) : base($"Log Com o Id:^{Id} não encontrado.")
        {
        }
    }
}
