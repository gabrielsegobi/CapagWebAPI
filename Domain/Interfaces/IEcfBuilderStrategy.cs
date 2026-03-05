using Domain.Entities.Sped.Ecf;

namespace Domain.Interfaces
{
    public interface IEcfBuilderStrategy
    {
        string Codigo { get; }
        EcfBase Build(string linha, long Id, long IdOp, long? IdPai, long FileId, string Competencia);
    }
}
