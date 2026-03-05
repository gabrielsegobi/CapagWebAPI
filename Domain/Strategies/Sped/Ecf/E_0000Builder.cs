using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Strategies.Sped.Ecf
{
    public class E_0000Builder : IEcfBuilderStrategy
    {
        public string Codigo => "0000";

        public EcfBase Build(string linha, long Id, long IdOp, long? IdPai, long FileId, string Competencia)
        {
            var campos = linha.Split('|');

            var reg = campos[1];
            var nomeEsc = campos[2];
            var codVer = campos[3];
            var cnpj = campos[4];
            var nome = campos[5];
            var indSitIniPer = campos[6];
            var sitEspecial = campos[7];
            var patRemanCis = campos[8];

            DateTime? dtSitEsp = ConverterData(campos, 9);
            DateTime? dtIni = ConverterData(campos, 10);
            DateTime? dtFin = ConverterData(campos, 11);

            var retificadora = campos[12];
            var numRec = campos[13];
            var tipEcf = campos[14];
            var codScp = campos[15];




            return new E_0000(
                Id,
                IdPai,
                IdOp,
                FileId,
                reg,
                nomeEsc,
                codVer,
                cnpj,
                nome,
                indSitIniPer,
                sitEspecial,
                patRemanCis,
                dtSitEsp,
                dtIni,
                dtFin,
                retificadora,
                numRec,
                tipEcf,
                codScp
                );
        }


        private static DateTime? ConverterData(string[] campos, int index)
        {
            if (campos.Length <= index || string.IsNullOrWhiteSpace(campos[index]))
                return null;

            if (DateTime.TryParseExact(
                campos[index],
                "ddMMyyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var data))
            {
                return data;
            }

            return null;
        }
    }
}
