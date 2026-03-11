using Domain.Contracts.Responses;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Application.Strategies
{
    public class PjNOptanteCalculoStrategy : ICalculoGrupoStrategy
    {
        public string Tag => "pj_ativa_nao_optante";

        private readonly IBaseRepository<RegDarfs> _darfRepository;
        private readonly IBaseRepository<RegDirfTerceiro> _dirfRepository;
        private readonly IBaseRepository<RegDctf> _dctfRepository;
        private readonly IBaseRepository<DemonstrativoContabil> _dreRepository;

        public PjNOptanteCalculoStrategy(IBaseRepository<RegDarfs> darfRepository, IBaseRepository<RegDirfTerceiro> dirfRepository, IBaseRepository<RegDctf> dctfRepository, IBaseRepository<DemonstrativoContabil> dreRepository)
        {
            _darfRepository = darfRepository;
            _dirfRepository = dirfRepository;
            _dctfRepository = dctfRepository;
            _dreRepository = dreRepository;
        }

        public async Task<GetApiResponse> CalcularAsync(long idEmpresa, string ano, CancellationToken cancellationToken)
        {
            string[] codigos = { "1708", "3280", "5944", "8045" };
            string codigoDre = "3.01.01.01.01";

            var darfs = await _darfRepository.Query()
                .Where(d =>
                    d.IdEmpresa == idEmpresa &&
                    d.DataArrecadacao.Year.ToString() == ano
                )
                 .SumAsync(d => d.ValorTotal, cancellationToken);

            var rendimento = await _dirfRepository.Query()
                .Where(d =>
                    d.IdEmpresa == idEmpresa &&
                    d.AnoCalendario.ToString() == ano &&
                    codigos.Contains(d.Codigo)
                )
                .SumAsync(d => d.ValorRendimento, cancellationToken);

            var tributo = await _dirfRepository.Query()
                .Where(d =>
                    d.IdEmpresa == idEmpresa &&
                    d.AnoCalendario.ToString() == ano 
                )
                .SumAsync(d => d.ValorTributo, cancellationToken);

            //var totalDctf = await _dctfRepository.Query()
            //    .Where(d =>
            //        d.IdEmpresa == idEmpresa
            //        && d.Periodo.StartsWith(ano)

            //    )
            //    .SumAsync(d => d.Valor, cancellationToken);

         

            var dctf = await _dctfRepository.Query()
               .Where(d =>
                   d.IdEmpresa == idEmpresa &&
                   d.Periodo.StartsWith(ano)
               )
               .GroupBy(_ => 1)
               .Select(g => new
               {
                   TotalValor = g.Sum(x => x.Valor),
                   TotalArquivos = g.Select(x => x.IdFilename).Distinct().Count()
               })
               .FirstOrDefaultAsync(cancellationToken);
            //var dre = await _dreRepository.Query()
            //    .FirstOrDefaultAsync(d =>
            //        d.IdEmpresa == idEmpresa &&
            //        d.Ano.ToString() == ano &&
            //        d.Codigo == codigoDre
            //    );


            var dre = await _dreRepository.Query()
                .Where(d =>
                    d.IdEmpresa == idEmpresa &&
                    d.Ano.ToString() == ano &&
                    d.Codigo == codigoDre
                )
                .ToListAsync();


            var registroAnual = dre.FirstOrDefault(d => d.PerApur.StartsWith("A"));
            var registrosTrimestrais = dre
                .Where(d => d.PerApur.StartsWith("T"))
                .OrderBy(d => d.PerApur) 
                .ToList();

            decimal valCtaRefFin;

            if (registroAnual != null)
            {
                valCtaRefFin = registroAnual.ValCtaRefFin.GetValueOrDefault();
            }
            else if (registrosTrimestrais.Any())
            {
                valCtaRefFin = registrosTrimestrais.Sum(d => d.ValCtaRefFin.GetValueOrDefault());
            }
            else
            {
                valCtaRefFin = 0;
            }


            // Decide o valor final
            //decimal valCtaRefFin = registroAnual != null
            //    ? registroAnual.ValCtaRefFin         // usa o anual se existir
            //    : registrosTrimestrais.Sum(d => d.ValCtaRefFin); // soma trimestrais se não houver anual

            // se encntrar o t deve somar todos os anos trimestres 
            //se emncontrar o a deve somar apenas a pirmeira emrpesa

            // registrar o peridodo do dctf e fazer por ao


            return new GetApiResponse
            {
                Data = new
                {
                    v1 = darfs,
                    v2 = rendimento,
                    v3 = tributo,
                    v6 = valCtaRefFin,
                    v7 = dctf?.TotalValor ?? 0,
                    count_files = new
                    {
                        v7 = dctf?.TotalArquivos ?? 0
                    }

                }
            };


            //.Where(d =>
            //    d.IdEmpresa == idEmpresa &&
            //    d.DataArrecadacao == dataArrecadacao
            //)
            //.ToListAsync(cancellationToken);
        }
    }
}
