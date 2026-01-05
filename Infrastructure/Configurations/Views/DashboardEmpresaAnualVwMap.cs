using Domain.Entities.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Views
{
    public class DashboardEmpresaAnualVwMap : IEntityTypeConfiguration<DashboardEmpresaAnualVw>
    {
        public void Configure(EntityTypeBuilder<DashboardEmpresaAnualVw> builder)
        {
            builder.ToView("vw_dashboard_empresa_anual");
            builder.HasNoKey();

            builder.Property(v => v.IdTenant).HasColumnName("id_tenant");
            builder.Property(v => v.IdEmpresa).HasColumnName("id_empresa");
            builder.Property(v => v.Cnpj).HasColumnName("cnpj");
            builder.Property(v => v.RazaoSocial).HasColumnName("razao_social");
            builder.Property(v => v.Ano).HasColumnName("ano");
            builder.Property(v => v.IcpCalculado).HasColumnName("icp_calculado");
            builder.Property(v => v.Classificacao).HasColumnName("classificacao");
            builder.Property(v => v.MargemLiquida).HasColumnName("margem_liquida");
            builder.Property(v => v.Roe).HasColumnName("roe");
            builder.Property(v => v.LiquidezCorrente).HasColumnName("liquidez_corrente");
        }
    }
}
