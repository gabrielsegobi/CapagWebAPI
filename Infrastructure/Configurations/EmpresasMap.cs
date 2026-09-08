using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class EmpresasMap : IEntityTypeConfiguration<Empresa>
    {
        public void Configure(EntityTypeBuilder<Empresa> builder)
        {
            builder.ToTable("empresas");
            builder.HasKey(e => e.IdEmpresa);

            builder.Property(e => e.IdEmpresa).UseMySqlIdentityColumn().HasColumnName("id_empresa").HasColumnType("BIGINT");
            builder.Property(e => e.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.Cnpj).HasColumnName("cnpj").HasColumnType("VARCHAR(14)").IsRequired();
            builder.Property(e => e.RazaoSocial).HasColumnName("razao_social").HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(e => e.NomeFantasia).HasColumnName("nome_fantasia").HasColumnType("VARCHAR(255)").IsRequired();
            //builder.Property(e => e.MatrizFilial).HasColumnName("matriz_filial").HasColumnType("ENUM('matriz','filial')").HasConversion(
            //v => v.ToString(),
            //v => (MatrizFilialEnum)Enum.Parse(typeof(MatrizFilialEnum), v)).IsRequired();
            builder.Property(e => e.MatrizFilial).HasColumnName("matriz_filial").HasColumnType("VARCHAR(255)").IsRequired();
            builder.Property(e => e.IdEmpresaMatriz).HasColumnName("id_empresa_matriz").HasColumnType("BIGINT");
            builder.Property(e => e.Ativa).HasColumnName("ativa").HasColumnType("TINYINT(1)").IsRequired();
            builder.Property(e => e.CreatedAt).HasColumnName("created_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(e => e.UpdatedAt).HasColumnName("updated_at").HasColumnType("TIMESTAMP").IsRequired();
            builder.Property(e => e.DeletedAt).HasColumnName("deleted_at").HasColumnType("TIMESTAMP");
            builder.Property(e => e.DadosProcessados).HasColumnName("dados_processados").HasColumnType("TINYINT(1)").IsRequired();
            builder.Property(e => e.Cnae).HasColumnName("cnae").HasColumnType("VARCHAR(150)").IsRequired();
            builder.Property(e => e.MunicipioEstado).HasColumnName("municipio_estado").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(e => e.DataAbertura).HasColumnName("data_abertura").HasColumnType("TIMESTAMP");
            builder.Property(e => e.CapitalSocial).HasColumnName("capital_social").HasColumnType("VARCHAR(50)").IsRequired();
            builder.Property(e => e.Segmento).HasColumnName("segmento").HasColumnType("VARCHAR(100)").IsRequired();
            builder.Property(e => e.Porte).HasColumnName("porte").HasColumnType("VARCHAR(30)").IsRequired();
            builder.Property(e => e.DataImpedimento).HasColumnName("data_impedimento").HasColumnType("TIMESTAMP");
            builder.Property(e => e.DataProtocolo).HasColumnName("data_protocolo").HasColumnType("DATE");
            builder.Property(e => e.Status).HasColumnName("status").HasColumnType("VARCHAR(30)");
            builder.Property(e => e.ValorContrato).HasColumnName("valor_contrato").HasColumnType("DECIMAL(20,2)");
            builder.Property(e => e.IdUsuarioResponsavel).HasColumnName("id_usuario_responsavel").HasColumnType("BIGINT");
        }
    }
}
