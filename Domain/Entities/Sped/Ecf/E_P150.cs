using System.Text;

namespace Domain.Entities.Sped.Ecf
{
    public class E_P150 : EcfBase
    {
        public E_P150(
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
            string valor,
            string indValor
           
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
            Valor = valor;
            IndValor = indValor;
        }

        public string Reg { get; private set; } = string.Empty;
        public string Codigo { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public string Tipo { get; private set; } = string.Empty;
        public string Nivel { get; private set; } = string.Empty;
        public string CodNat { get; private set; } = string.Empty;
        public string CodCtaSup { get; private set; } = string.Empty;
        public string Valor { get; private set; } = string.Empty;
        public string IndValor { get; private set; } = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("E_P150 {");
            sb.AppendLine($"  Reg = {Reg}");
            sb.AppendLine($"  Codigo = {Codigo}");
            sb.AppendLine($"  Descricao = {Descricao}");
            sb.AppendLine($"  Tipo = {Tipo}");
            sb.AppendLine($"  Nivel = {Nivel}");
            sb.AppendLine($"  CodNat = {CodNat}");
            sb.AppendLine($"  CodCtaSup = {CodCtaSup}");
            sb.AppendLine($"  Valor = {Valor}");
            sb.AppendLine($"  IndValor = {IndValor}");
            sb.AppendLine($"  FileName = {FileName}");
            sb.AppendLine($"  FileNameId = {FileId}");
            sb.AppendLine($"  IdOp = {IdOp}");
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}