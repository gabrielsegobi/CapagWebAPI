using System.Text;

namespace Domain.Entities.Sped.Ecf
{
    public class E_0000 : EcfBase
    {
        //public E_0000(long id, long idPai, long idTenant, long idEmpresa): base(id, idPai, idTenant, idEmpresa)
        //{
        //}

        public E_0000(long id, long? idPai, long idTenant, long idEmpresa, long idOp, long fileId, string fileName, string reg, string nomeEsc, string codVer, string cnpj, string nome, string indSitIniPer, string sitEspecial,
            string patRemanCis, DateTime? dtSitEsp, DateTime? dtIni, DateTime? dtFin, string retificadora,
            string numRec, string tipEcf, string codScp) : base(id, idPai, idTenant, idEmpresa, idOp, fileId, fileName)
        {
            Reg = reg;
            NomeEsc = nomeEsc;
            CodVer = codVer;
            Cnpj = cnpj;
            Nome = nome;
            IndSitIniPer = indSitIniPer;
            SitEspecial = sitEspecial;
            PatRemanCis = patRemanCis;
            DtSitEsp = dtSitEsp;
            DtIni = dtIni;
            DtFin = dtFin;
            Retificadora = retificadora;
            NumRec = numRec;
            TipEcf = tipEcf;
            CodScp = codScp;
        }

        public string Reg { get; private set; } = string.Empty;
        public string NomeEsc { get; private set; } = string.Empty;
        public string CodVer { get; private set; } = string.Empty;
        public string Cnpj { get; private set; } = string.Empty;
        public string Nome { get; private set; } = string.Empty;
        public string IndSitIniPer { get; private set; } = string.Empty;
        public string SitEspecial { get; private set; } = string.Empty;
        public string PatRemanCis { get; private set; } = string.Empty;

        public DateTime? DtSitEsp { get; private set; }
        public DateTime? DtIni { get; private set; }
        public DateTime? DtFin { get; private set; }

        public string Retificadora { get; private set; } = string.Empty;
        public string NumRec { get; private set; } = string.Empty;
        public string TipEcf { get; private set; } = string.Empty;
        public string CodScp { get; private set; } = string.Empty;

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("E_0000 {");
            sb.AppendLine($"  Reg = {Reg}");
            sb.AppendLine($"  NomeEsc = {NomeEsc}");
            sb.AppendLine($"  CodVer = {CodVer}");
            sb.AppendLine($"  Cnpj = {Cnpj}");
            sb.AppendLine($"  Nome = {Nome}");
            sb.AppendLine($"  IndSitIniPer = {IndSitIniPer}");
            sb.AppendLine($"  SitEspecial = {SitEspecial}");
            sb.AppendLine($"  PatRemanCis = {PatRemanCis}");
            sb.AppendLine($"  DtSitEsp = {(DtSitEsp.HasValue ? DtSitEsp.Value.ToString("dd/MM/yyyy") : "null")}");
            sb.AppendLine($"  DtIni = {(DtIni.HasValue ? DtIni.Value.ToString("dd/MM/yyyy") : "null")}");
            sb.AppendLine($"  DtFin = {(DtFin.HasValue ? DtFin.Value.ToString("dd/MM/yyyy") : "null")}");
            sb.AppendLine($"  Retificadora = {Retificadora}");
            sb.AppendLine($"  NumRec = {NumRec}");
            sb.AppendLine($"  TipEcf = {TipEcf}");
            sb.AppendLine($"  CodScp = {CodScp}");
            sb.AppendLine($"  FileName = {FileName}");
            sb.AppendLine($"  FileNameId = {FileId}");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }


}
