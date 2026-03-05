using System;
using System.Text;

namespace Domain.Entities.Sped.Ecf
{
    public class E_P001 : EcfBase
    {
        public E_P001(
            long id,
            long? idPai,
            long idOp,
            long fileId,
            string reg,
            string indDad
        ) : base(id, idPai, idOp, fileId)
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
            sb.AppendLine($" IdOp = {IdOp}");
            sb.AppendLine($" FileNameId = {FileId}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
