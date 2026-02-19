namespace Domain.Entities.Sped.Ecf
{
    public abstract class EcfBase
    {
        protected EcfBase(long id, long? idPai, long idTenant, long idEmpresa, long idOp, long fileId, string fileName)
        {
            Id = id;
            IdPai = idPai;
            IdTenant = idTenant;
            IdEmpresa = idEmpresa;
            IdOp = idOp;
            FileId = fileId;
            FileName = fileName;
        }

        public long Id { get; set; }
        public long? IdPai { get; set; }
        public long IdTenant { get; set; }
        public long IdEmpresa { get; set; }
        public long IdOp { get; set; }
        public string FileName { get; set; } = string.Empty;
        public long FileId { get; set; }

    }
}
