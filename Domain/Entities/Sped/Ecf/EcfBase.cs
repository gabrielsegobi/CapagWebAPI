namespace Domain.Entities.Sped.Ecf
{
    public abstract class EcfBase
    {
        protected EcfBase(long id, long? idPai, long idOp, long fileId)
        {
            Id = id;
            IdPai = idPai;
            IdOp = idOp;
            FileId = fileId;
        }

        public long Id { get; set; }
        public long? IdPai { get; set; }
        public long IdOp { get; set; }
        public long FileId { get; set; }
    }
}
