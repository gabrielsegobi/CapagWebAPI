using Domain.Entities.Sped.Ecf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Sped.Ecf
{
    public class E_0000Map : IEntityTypeConfiguration<E_0000>
    {
        public void Configure(EntityTypeBuilder<E_0000> builder)
        {
            builder.ToTable("ecf_0000");

            builder.HasKey(e => new { e.FileId, e.Id, });


            // ===== EcfBase =====
            builder.Property(e => e.Id).HasColumnName("id").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdOp).HasColumnName("id_op").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdPai).HasColumnName("id_pai").HasColumnType("BIGINT");
            //builder.Property(e => e.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            //builder.Property(e => e.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.FileId).HasColumnName("file_id").HasColumnType("BIGINT").IsRequired();
            //builder.Property(e => e.FileName).HasColumnName("file_name").HasColumnType("VARCHAR(255)");

            // ===== E_0000 =====
            builder.Property(e => e.Reg).HasColumnName("reg").HasColumnType("TEXT");
            builder.Property(e => e.NomeEsc).HasColumnName("nome_esc").HasColumnType("TEXT");
            builder.Property(e => e.CodVer).HasColumnName("cod_ver").HasColumnType("TEXT");
            builder.Property(e => e.Cnpj).HasColumnName("cnpj").HasColumnType("TEXT");
            builder.Property(e => e.Nome).HasColumnName("nome").HasColumnType("TEXT");
            builder.Property(e => e.IndSitIniPer).HasColumnName("ind_sit_ini_per").HasColumnType("TEXT");
            builder.Property(e => e.SitEspecial).HasColumnName("sit_especial").HasColumnType("TEXT");
            builder.Property(e => e.PatRemanCis).HasColumnName("pat_reman_cis").HasColumnType("TEXT");
            builder.Property(e => e.DtSitEsp).HasColumnName("dt_sit_esp").HasColumnType("DATE");
            builder.Property(e => e.DtIni).HasColumnName("dt_ini").HasColumnType("DATE");
            builder.Property(e => e.DtFin).HasColumnName("dt_fin").HasColumnType("DATE");
            builder.Property(e => e.Retificadora).HasColumnName("retificadora").HasColumnType("TEXT");
            builder.Property(e => e.NumRec).HasColumnName("num_rec").HasColumnType("TEXT");
            builder.Property(e => e.TipEcf).HasColumnName("tip_ecf").HasColumnType("TEXT");
            builder.Property(e => e.CodScp).HasColumnName("cod_scp").HasColumnType("TEXT");
        }
    }
}
