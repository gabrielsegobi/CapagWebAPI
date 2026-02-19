using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;

namespace Domain.Strategies.Sped.Ecf
{
    public class E_P001builder : IEcfBuilderStrategy
    {
        public string Codigo => "P001";

        public EcfBase Build(string linha, long Id, long IdOp, long IdTenant, long IdEmpresa, long? IdPai, long FileId, string FileName, string Competencia)
        {
            var campos = linha.Split('|');

            var reg = campos[1];
            var indDad = campos[2];

            return new E_P001(
                Id,
                IdPai,
                IdTenant,
                IdEmpresa,
                IdOp,
                FileId,
                FileName,
                reg,
                indDad
                );
        }
    }
}
