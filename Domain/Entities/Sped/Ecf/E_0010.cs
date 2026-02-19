using System.Text;

namespace Domain.Entities.Sped.Ecf
{
    public class E_0010 : EcfBase
    {
        public E_0010(
            long id,
            long? idPai,
            long idTenant,
            long idEmpresa,
            long idOp,
            string reg,
            string hashEcfAnterior,
            string optRefis,
            string optPaes,
            string formaTrib,
            string formaApur,
            string codQualifPj,
            string formaTribPer,
            string mesBalRed,
            string tipEscPre,
            string tipEnt,
            string formaApurI,
            string apurCsll,
            string optExtRtt,
            string difFcont,
            string indRecReceita,
            string fileName,
            long fileId
        ) : base(id, idPai, idTenant, idEmpresa, idOp, fileId, fileName)
        {
            Reg = reg;
            HashEcfAnterior = hashEcfAnterior;
            OptRefis = optRefis;
            OptPaes = optPaes;
            FormaTrib = formaTrib;
            FormaApur = formaApur;
            CodQualifPj = codQualifPj;
            FormaTribPer = formaTribPer;
            MesBalRed = mesBalRed;
            TipEscPre = tipEscPre;
            TipEnt = tipEnt;
            FormaApurI = formaApurI;
            ApurCsll = apurCsll;
            OptExtRtt = optExtRtt;
            DifFcont = difFcont;
            IndRecReceita = indRecReceita;
        }

        public string? Reg { get; private set; } = string.Empty;
        public string? HashEcfAnterior { get; private set; } = string.Empty;
        public string? OptRefis { get; private set; } = string.Empty;
        public string? OptPaes { get; private set; } = string.Empty;
        public string? FormaTrib { get; private set; } = string.Empty;
        public string? FormaApur { get; private set; } = string.Empty;
        public string? CodQualifPj { get; private set; } = string.Empty;
        public string? FormaTribPer { get; private set; } = string.Empty;
        public string? MesBalRed { get; private set; } = string.Empty;
        public string TipEscPre { get; private set; } = string.Empty;
        public string? TipEnt { get; private set; } = string.Empty;
        public string? FormaApurI { get; private set; } = string.Empty;
        public string? ApurCsll { get; private set; } = string.Empty;
        public string? OptExtRtt { get; private set; } = string.Empty;
        public string? DifFcont { get; private set; } = string.Empty;
        public string? IndRecReceita { get; private set; } = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("E_0010 {");
            sb.AppendLine($" Reg = {Reg}");
            sb.AppendLine($" HashEcfAnterior = {HashEcfAnterior}");
            sb.AppendLine($" OptRefis = {OptRefis}");
            sb.AppendLine($" OptPaes = {OptPaes}");
            sb.AppendLine($" FormaTrib = {FormaTrib}");
            sb.AppendLine($" FormaApur = {FormaApur}");
            sb.AppendLine($" CodQualifPj = {CodQualifPj}");
            sb.AppendLine($" FormaTribPer = {FormaTribPer}");
            sb.AppendLine($" AesBalRed = {MesBalRed}");
            sb.AppendLine($" TipEscPre = {TipEscPre}");
            sb.AppendLine($" TipEnt = {TipEnt}");
            sb.AppendLine($" FormaApurI = {FormaApurI}");
            sb.AppendLine($" ApurCsll = {ApurCsll}");
            sb.AppendLine($" OptExtRtt = {OptExtRtt}");
            sb.AppendLine($" DifFcont = {DifFcont}");
            sb.AppendLine($" IndRecReceita = {IndRecReceita}");
            sb.AppendLine($" FileName = {FileName}");
            sb.AppendLine($" FileNameId = {FileId}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}
