namespace Domain.Interfaces
{
    public interface IEcfProcessorService
    {
        Task ProcessAsync(long IdOp, long IdEmpresa, long IdTenant, long FileId, string FileName, CancellationToken cancellationToken, bool overwrite = false);
    }
}
