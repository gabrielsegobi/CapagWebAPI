using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;
using System.Globalization;

namespace Domain.Strategies.Sped.Ecf
{
    public class E_0010Builder : IEcfBuilderStrategy
    {
        public string Codigo => "0010";

        public EcfBase Build(string linha, long Id, long IdOp, long IdTenant, long IdEmpresa, long? IdPai, long FileId, string FileName, string Competencia)
        {
            var campos = linha.Split('|');
            var reg = campos[1];
            var hashEcfAnterior = campos[2];
            var optRefis = campos[3];
            var optPaes = campos[4];
            var FormaTrib = campos[5];
            var FormaApur = campos[6];
            var CodQualifPj = campos[7];
            var FormaTribPer = campos[8];
            var MesBalRed = campos[9];
            var TipEscPre = campos[10];
            var TipEnt = campos[11];
            var formaApurI = campos[12];
            var apurCsll = campos[13];
            //var optExtRtt = campos[14];
            //var difFcont = campos[15];

            string optExtRtt = null;
            string difFcont = null;
            string indReceita = null;

            var competenciaDt = DateTime.ParseExact(
                Competencia,
                "yyyyMM",
                CultureInfo.InvariantCulture
            );

            // Até 31/12/2014
            if (competenciaDt <= new DateTime(2014, 12, 31))
            {
                optExtRtt = campos.Length > 14 ? campos[14] : null;
                difFcont = campos.Length > 15 ? campos[15] : null;
            }
            // A partir de 01/01/2016
            else if (competenciaDt >= new DateTime(2016, 1, 1))
            {
                indReceita = campos.Length > 14 ? campos[14] : null;
            }

            return new E_0010(
                Id,
                IdPai,
                IdTenant,
                IdEmpresa,
                IdOp,
                reg,
                hashEcfAnterior,
                optRefis,
                optPaes,
                FormaTrib,
                FormaApur,
                CodQualifPj,
                FormaTribPer,
                MesBalRed,
                TipEscPre,
                TipEnt,
                formaApurI,
                apurCsll,
                optExtRtt,
                difFcont,
               
                indReceita,
                 FileName,
                FileId
             );

        }
    }
}
