using System;
using System.Text;

namespace Domain.Entities.Sped.Ecf
{
    public class E_P001 : EcfBase
    {
        public E_P001(
            long id,
            long? idPai,
            long idTenant,
            long idEmpresa,
            long idOp,
            long fileId,
            string fileName,
            string reg,
            string indDad
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
            sb.AppendLine("E_P001 {");
            sb.AppendLine($" Reg = {Reg}");
            sb.AppendLine($" IndDad = {IndDad}");
            sb.AppendLine($" FileName = {FileName}");
            sb.AppendLine($" IdOp = {IdOp}");
            sb.AppendLine($" FileNameId = {FileId}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
