using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Application.Helpers
{
    public static class LimparCalculosEmpresaHelper
    {
        public static async Task LimparIndicadoresAnterioresAsync(
            IBaseRepository<Indicador> indicadorRepo,
            IBaseRepository<ValorAnual> valorAnualRepo,
            long idEmpresa,
            long idTenant,
            CancellationToken cancellationToken)
        {
            var now = DateTimeHelper.GetDateTimeNow();

            var indicadores = await indicadorRepo
                .Query(i => i.IdEmpresa == idEmpresa &&
                            i.IdTenant == idTenant &&
                            i.DeletedAt == null,
                    asNoTracking: false)
                .ToListAsync(cancellationToken);

            if (indicadores.Count == 0)
                return;

            var idsIndicadores = indicadores.Select(i => i.IdIndicador).ToList();

            var valoresAnuais = await valorAnualRepo
                .Query(v => idsIndicadores.Contains(v.IdIndicador), asNoTracking: false)
                .ToListAsync(cancellationToken);

            if (valoresAnuais.Count > 0)
            {
                valorAnualRepo.DeleteRange(valoresAnuais);
                await valorAnualRepo.SaveChangesAsync();
            }

            foreach (var indicador in indicadores)
            {
                indicador.DeletedAt = now;
                indicador.UpdatedAt = now;
            }

            indicadorRepo.UpdateRange(indicadores);
            await indicadorRepo.SaveChangesAsync();
        }

        public static async Task LimparResultadosIcpAnterioresAsync(
            IBaseRepository<ResultadoIndiceICP> resultadoIndiceIcpRepo,
            IBaseRepository<AnaliseICP> analiseIcpRepo,
            long idEmpresa,
            long idTenant,
            CancellationToken cancellationToken)
        {
            var now = DateTimeHelper.GetDateTimeNow();

            var resultados = await resultadoIndiceIcpRepo
                .Query(r => r.IdEmpresa == idEmpresa && r.IdTenant == idTenant, asNoTracking: false)
                .ToListAsync(cancellationToken);

            if (resultados.Count > 0)
            {
                resultadoIndiceIcpRepo.DeleteRange(resultados);
                await resultadoIndiceIcpRepo.SaveChangesAsync();
            }

            var analises = await analiseIcpRepo
                .Query(a => a.IdEmpresa == idEmpresa &&
                            a.IdTenant == idTenant &&
                            a.DeletedAt == null,
                    asNoTracking: false)
                .ToListAsync(cancellationToken);

            if (analises.Count == 0)
                return;

            foreach (var analise in analises)
            {
                analise.DeletedAt = now;
                analise.UpdatedAt = now;
            }

            analiseIcpRepo.UpdateRange(analises);
            await analiseIcpRepo.SaveChangesAsync();
        }
    }
}
