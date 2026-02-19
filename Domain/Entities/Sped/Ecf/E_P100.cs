using System.Text;

namespace Domain.Entities.Sped.Ecf
{
    public class E_P100 : EcfBase
    {
        public E_P100(
            long id,
            long? idPai,
            long idTenant,
            long idEmpresa,
            long idOp,
            long fileId,
            string fileName,
            string reg,
            string codigo,
            string descricao,
            string tipo,
            string nivel,
            string codNat,
            string codCtaSup,
            string valCtaRefIni,
            string indValCtaRefIni,
            string valCtaRefDeb,
            string valCtaRefCred,
            string valCtaRefFin,
            string indValCtaRefFin
            )
            : base(id, idPai, idTenant, idEmpresa, idOp, fileId, fileName)
        {
            Reg = reg;
            Codigo = codigo;
            Descricao = descricao;
            Tipo = tipo;
            Nivel = nivel;
            CodNat = codNat;
            CodCtaSup = codCtaSup;
            ValCtaRefIni = valCtaRefIni;
            IndValCtaRefIni = indValCtaRefIni;
            ValCtaRefDeb = valCtaRefDeb;
            ValCtaRefCred = valCtaRefCred;
            ValCtaRefFin = valCtaRefFin;
            IndValCtaRefFin = indValCtaRefFin;
        }

        public string Reg { get; private set; } = string.Empty;
        public string Codigo { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public string Tipo { get; private set; } = string.Empty;
        public string Nivel { get; private set; } = string.Empty;
        public string CodNat { get; private set; } = string.Empty;
        public string CodCtaSup { get; private set; } = string.Empty;
        public string ValCtaRefIni { get; private set; } = string.Empty;
        public string IndValCtaRefIni { get; private set; } = string.Empty;
        public string ValCtaRefDeb { get; private set; } = string.Empty;
        public string ValCtaRefCred { get; private set; } = string.Empty;
        public string ValCtaRefFin { get; private set; } = string.Empty;
        public string IndValCtaRefFin { get; private set; } = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("E_P100 {");
            sb.AppendLine($"  Reg = {Reg}");
            sb.AppendLine($"  Codigo = {Codigo}");
            sb.AppendLine($"  Descricao = {Descricao}");
            sb.AppendLine($"  Tipo = {Tipo}");
            sb.AppendLine($"  Nivel = {Nivel}");
            sb.AppendLine($"  CodNat = {CodNat}");
            sb.AppendLine($"  CodCtaSup = {CodCtaSup}");
            sb.AppendLine($"  ValCtaRefIni = {ValCtaRefIni}");
            sb.AppendLine($"  IndValCtaRefIni = {IndValCtaRefIni}");
            sb.AppendLine($"  ValCtaRefDeb = {ValCtaRefDeb}");
            sb.AppendLine($"  ValCtaRefCred = {ValCtaRefCred}");
            sb.AppendLine($"  ValCtaRefFin = {ValCtaRefFin}");
            sb.AppendLine($"  IndValCtaRefFin = {IndValCtaRefFin}");
            sb.AppendLine($"  FileName = {FileName}");
            sb.AppendLine($"  FileNameId = {FileId}");
            sb.AppendLine($"  IdOp = {IdOp}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}