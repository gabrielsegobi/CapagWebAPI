using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class AuditLogMap : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("audit_log");
            builder.HasKey(a => a.Id);

            builder.Property(al => al.Id).UseMySqlIdentityColumn().HasColumnName("id").HasColumnType("BIGINT");
            builder.Property(al => al.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(al => al.IdUsuario).HasColumnName("id_usuario").HasColumnType("BIGINT"); 
            builder.Property(al => al.Tabela).HasColumnName("tabela").HasColumnType("VARCHAR(64)").IsRequired();
            builder.Property(al => al.IdRegistro).HasColumnName("id_registro").HasColumnType("BIGINT").IsRequired();
            //builder.Property(al => al.Acao).HasColumnName("acao").HasColumnType("ENUM('INSERT','UPDATE','DELETE')").HasConversion(
            // v => v.ToString(),
            // v => (AcaoEnum)Enum.Parse(typeof(AcaoEnum), v)).IsRequired();
            builder.Property(al => al.Acao).HasColumnName("acao").HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(al => al.DadosAntigos).HasColumnName("dados_antigos").HasColumnType("json");
            builder.Property(al => al.DadosNovos).HasColumnName("dados_novos").HasColumnType("json");
            builder.Property(al => al.IpAddress).HasColumnName("ip_address").HasColumnType("VARCHAR(45)");
            builder.Property(al => al.UserAgent).HasColumnName("user_agent").HasColumnType("TEXT");
            builder.Property(al => al.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(a => a.CreatedYear) .HasColumnName("created_year").HasComputedColumnSql("YEAR(created_at)").ValueGeneratedOnAddOrUpdate();
        }
    }
}
