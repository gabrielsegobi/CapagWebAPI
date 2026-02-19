using System.Text;

namespace Domain.Entities.Sped.Ecf
{
    public class E_L030 : EcfBase
    {
        public E_L030(
            long id,
            long? idPai,
            long idTenant,
            long idEmpresa,
            long idOp,
            long fileId,
            string fileName,
            string reg,
            string dtIni,
            string dtFin,
            string perApur
        ) : base(id, idPai, idTenant, idEmpresa, idOp, fileId, fileName)
        {
            Reg = reg;
            DtIni = dtIni;
            DtFin = dtFin;
            PerApur = perApur;
        }

        public string Reg { get; private set; } = string.Empty;
        public string DtIni { get; private set; } = string.Empty;
        public string DtFin { get; private set; } = string.Empty;
        public string PerApur { get; private set; } = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("E_1030 {");
            sb.AppendLine($" Reg = {Reg}");
            sb.AppendLine($" DtIni = {DtIni}");
            sb.AppendLine($" DtFin = {DtFin}");
            sb.AppendLine($" PerApur = {PerApur}");
            sb.AppendLine($" FileName = {FileName}");
            sb.AppendLine($" FileNameId = {FileId}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
