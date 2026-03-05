using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;

namespace Domain.Strategies.Sped.Ecf
{
    public class E_L300Builder : IEcfBuilderStrategy
    {
        public string Codigo => "L300";

        public EcfBase Build(string linha, long Id, long IdOp, long? IdPai, long FileId, string Competencia)
        {
            var campos = linha.Split('|');

            var reg = campos[1];
            var codigo = campos[2];
            var descricao = campos[3];
            var tipo = campos[4];
            var nivel = campos[5];
            var codNat = campos[6];
            var codCtaSup = campos[7];
            var valor = campos[8];
            var indValor = campos[9];

            return new E_L300(
                Id,
                IdPai,
                IdOp,
                FileId,
                reg,
                codigo,
                descricao,
                tipo,
                nivel,
                codNat,
                codCtaSup,
                valor,
                indValor
                );
        }
    }
}
