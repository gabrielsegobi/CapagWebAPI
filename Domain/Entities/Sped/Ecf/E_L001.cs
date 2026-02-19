using System.Text;

namespace Domain.Entities.Sped.Ecf
{
    public class E_L001 : EcfBase
    {
        public E_L001(
            long id,
            long? idPai,
            long idTenant,
            long idEmpresa,
            long idOp,
            string reg,
            string indDad,
            string fileName,
            long fileId
        ) : base(id, idPai, idTenant, idEmpresa, idOp, fileId, fileName)
        {
            Reg = reg;
            IndDad = indDad;
        }

        public string Reg { get; private set; } = string.Empty;
        public string IndDad { get; private set; } = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("E_1001 {");
            sb.AppendLine($" Reg = {Reg}");
            sb.AppendLine($" IndDad = {IndDad}");
            sb.AppendLine($" FileName = {FileName}");
            sb.AppendLine($" FileNameId = {FileId}");
            sb.AppendLine($" IDpai = {IdPai}");
            sb.AppendLine($" Id = {Id}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
