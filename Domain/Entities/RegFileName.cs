namespace Domain.Entities
{
    public class RegFileName
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        //public ICollection<RegIrpf> RegIrpfs { get; set; } = new List<RegIrpf>();
    }
}
