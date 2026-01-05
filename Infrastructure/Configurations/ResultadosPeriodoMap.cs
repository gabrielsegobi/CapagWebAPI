using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class ResultadosPeriodoMap : IEntityTypeConfiguration<ResultadosPeriodo>
    {
        public void Configure(EntityTypeBuilder<ResultadosPeriodo> builder)
        {
            builder.ToTable("resultados_periodo");
            builder.HasKey(rp => rp.Id);

            builder.Property(rp => rp.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(rp => rp.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(rp => rp.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(rp => rp.DtIni).HasColumnName("dt_ini").HasColumnType("DATE");
            builder.Property(rp => rp.DtIniApur).HasColumnName("dt_ini_apur").HasColumnType("DATE");
            builder.Property(rp => rp.DtFinApur).HasColumnName("dt_fin_apur").HasColumnType("DATE");
            builder.Property(rp => rp.PerApur).HasColumnName("per_apur").HasColumnType("VARCHAR(10)").IsRequired();
            builder.Property(rp => rp.Ano).HasColumnName("ano").HasColumnType("INT").IsRequired();
            builder.Property(rp => rp.Codigo).HasColumnName("codigo").HasColumnType("VARCHAR(20)").IsRequired();
            builder.Property(rp => rp.Descricao).HasColumnName("descricao").HasColumnType("VARCHAR(255)");
            builder.Property(rp => rp.Tipo).HasColumnName("tipo").HasColumnType("CHAR(1)");
            builder.Property(rp => rp.Nivel).HasColumnName("nivel").HasColumnType("TINYINT UNSIGNED");
            builder.Property(rp => rp.Valor).HasColumnName("valor").HasColumnType("DECIMAL(18,2)");
            builder.Property(rp => rp.IndValor).HasColumnName("ind_valor").HasColumnType("CHAR(1)");
            builder.Property(rp => rp.TipoTrib).HasColumnName("tipo_trib").HasColumnType("VARCHAR(50)");
            builder.Property(rp => rp.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(rp => rp.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(rp => rp.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");
        }
    }
}
