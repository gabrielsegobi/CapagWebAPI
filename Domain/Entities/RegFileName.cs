using System.Collections;

namespace Domain.Entities
{
    public class RegFileName
    {
        public long Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        //public ICollection<RegIrpf> RegIrpfs { get; set; } = new List<RegIrpf>();
        public ICollection<RegDefi> RegDefis { get; set; } = [];
        public ICollection<RegDarfs> RegDarfs { get; set; } = [];
        public ICollection<RegDctf> RegDctfs { get; set; } = [];
        public ICollection<RegDirfTerceiro> RegDirfTerceiros { get; set; } = [];
        public ICollection<RegIrpf> RegIrpfs { get; set; } = [];
        public ICollection<RegPgdasd> RegPgdasds { get; set; } = [];
    }
}
