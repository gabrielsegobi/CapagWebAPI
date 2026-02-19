
using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;

namespace Domain.Strategies.Sped.Ecf
{
    public class E_L030Builder : IEcfBuilderStrategy
    {
        public string Codigo => "L030";

        public EcfBase Build(string linha, long Id, long IdOp, long IdTenant, long IdEmpresa, long? IdPai, long FileId, string FileName, string Competencia)
        {
            var campos = linha.Split('|');

            var reg = campos[1];
            var dtIni = campos[2];
            var DtFin = campos[3];
            var PerApur = campos[4];

            return new E_L030(
                Id,
                IdPai,
                IdTenant,
                IdEmpresa,
                IdOp,
                FileId,
                FileName,
                reg,
                dtIni,
                DtFin,
                PerApur
            );
        }
    }
}
