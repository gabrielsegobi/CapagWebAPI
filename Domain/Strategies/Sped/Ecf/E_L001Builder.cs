using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;

namespace Domain.Strategies.Sped.Ecf
{
    public class E_L001Builder : IEcfBuilderStrategy
    {
        public string Codigo => "L001";

        public EcfBase Build(string linha, long Id, long IdOp, long? IdPai, long FileId, string Competencia)
        {
            var campos = linha.Split('|');

            var reg = campos[1];
            var indDad = campos[2];

            return new E_L001(
                Id,
                IdPai,
                IdOp,
                reg,
                indDad,
                FileId
            );
        }
    }
}
