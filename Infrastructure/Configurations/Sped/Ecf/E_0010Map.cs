using Domain.Entities.Sped.Ecf;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations.Sped.Ecf
{
    public class E_0010Map : IEntityTypeConfiguration<E_0010>
    {
        public void Configure(EntityTypeBuilder<E_0010> builder)
        {
            builder.ToTable("ecf_0010");

            builder.HasKey(e => new { e.FileId, e.Id, });


            // ===== EcfBase =====
            builder.Property(e => e.Id).HasColumnName("id").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdOp).HasColumnName("id_op").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.IdPai).HasColumnName("id_pai").HasColumnType("BIGINT");
            //builder.Property(e => e.IdTenant).HasColumnName("id_tenant").HasColumnType("BIGINT").IsRequired();
            //builder.Property(e => e.IdEmpresa).HasColumnName("id_empresa").HasColumnType("BIGINT").IsRequired();
            builder.Property(e => e.FileId).HasColumnName("file_id").HasColumnType("BIGINT").IsRequired();
            //builder.Property(e => e.FileName).HasColumnName("filename").HasColumnType("TEXT");

            // ===== E_0010 =====
            builder.Property(e => e.Reg).HasColumnName("reg").HasColumnType("TEXT");
            builder.Property(e => e.HashEcfAnterior).HasColumnName("hash_ecf_anterior").HasColumnType("TEXT");
            builder.Property(e => e.OptRefis).HasColumnName("opt_refis").HasColumnType("TEXT");
            builder.Property(e => e.OptPaes).HasColumnName("opt_paes").HasColumnType("TEXT");
            builder.Property(e => e.FormaTrib).HasColumnName("forma_trib").HasColumnType("TEXT");
            builder.Property(e => e.FormaApur).HasColumnName("forma_apur").HasColumnType("TEXT");
            builder.Property(e => e.CodQualifPj).HasColumnName("cod_qualif_pj").HasColumnType("TEXT");
            builder.Property(e => e.FormaTribPer).HasColumnName("forma_trib_per").HasColumnType("TEXT");
            builder.Property(e => e.MesBalRed).HasColumnName("mes_bal_red").HasColumnType("TEXT");
            builder.Property(e => e.TipEscPre).HasColumnName("tip_esc_pre").HasColumnType("TEXT");
            builder.Property(e => e.TipEnt).HasColumnName("tip_ent").HasColumnType("TEXT");
            builder.Property(e => e.FormaApurI).HasColumnName("forma_apur_i").HasColumnType("TEXT");
            builder.Property(e => e.ApurCsll).HasColumnName("apur_csll").HasColumnType("TEXT");
            builder.Property(e => e.OptExtRtt).HasColumnName("opt_ext_rtt").HasColumnType("TEXT");
            builder.Property(e => e.DifFcont).HasColumnName("dif_fcont").HasColumnType("TEXT");
            builder.Property(e => e.IndRecReceita).HasColumnName("ind_rec_receita").HasColumnType("TEXT");
        }
    }
}
