using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;
using System.Globalization;

namespace Domain.Strategies.Sped.Ecf
{
    public class E_L100Builder : IEcfBuilderStrategy
    {
        public string Codigo => "L100";

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
            var valCtaRefIni = campos[8];
            var indValCtaRefIni = campos[9];

            //var valCtaRefDeb = campos[10];
            //var valCtaRefCred = campos[11];

            //var valCtaRefFin = campos[12];
            //var indValCtaRefFin = campos[13];



            string valCtaRefFin = null;
            string indValCtaRefFin = null;

            string valCtaRefDeb = null;
            string valCtaRefCred = null;

            var competenciaDt = DateTime.ParseExact(
                Competencia,
                "yyyyMM",
                CultureInfo.InvariantCulture
            );

            // Até 31/12/2014
            if (competenciaDt <= new DateTime(2014, 12, 31))
            {
                valCtaRefFin = campos[10];
                indValCtaRefFin = campos[11];
            }
            // A partir de 01/01/2016
            else if (competenciaDt >= new DateTime(2018, 1, 1))
            {

                valCtaRefDeb = campos[10];
                valCtaRefCred = campos[11];
                valCtaRefFin = campos[12];
                indValCtaRefFin = campos[13];
            }

            return new E_L100(
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
                valCtaRefIni,
                indValCtaRefIni,
                valCtaRefDeb,
                valCtaRefCred,
                valCtaRefFin,
                indValCtaRefFin
                );
        }
    }
}
