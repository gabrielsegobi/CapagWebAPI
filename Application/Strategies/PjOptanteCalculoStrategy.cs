using Domain.Contracts.Responses;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Application.Strategies
{
    public class PjOptanteCalculoStrategy : ICalculoGrupoStrategy
    {
        public string Tag => "pj_ativa_optante_simples";


        private readonly IBaseRepository<RegDarfs> _darfRepository;
        private readonly IBaseRepository<RegDirfTerceiro> _dirfRepository;
        private readonly IBaseRepository<RegDctf> _dctfRepository;
        private readonly IBaseRepository<RegPgdasd> _pgdasdRepository;

        public PjOptanteCalculoStrategy(IBaseRepository<RegDarfs> darfRepository, IBaseRepository<RegDirfTerceiro> dirfRepository, IBaseRepository<RegDctf> dctfRepository, IBaseRepository<RegPgdasd> pgdasdRepository)
        {
            _darfRepository = darfRepository;
            _dirfRepository = dirfRepository;
            _dctfRepository = dctfRepository;
            _pgdasdRepository = pgdasdRepository;
        }

        public async Task<GetApiResponse> CalcularAsync(long idEmpresa, string ano, CancellationToken cancellationToken)
        {
            string[] codigos2 = { "3249", "3251", "3426", "5232", "5273", "5557", "6800", "6813", "8468" };
            string[] codigos = { "1708", "3280", "5944", "8045" };

            //var darfs = await _darfRepository.Query()
            //   .Where(d =>
            //       d.IdEmpresa == idEmpresa &&
            //       d.DataArrecadacao.Year.ToString() == ano
            //   )
            //    .SumAsync(d => d.ValorTotal, cancellationToken);

            var darfs = await _darfRepository.Query()
            .Where(d =>
                d.IdEmpresa == idEmpresa &&
                d.DataArrecadacao.Year.ToString() == ano
            )
            .GroupBy(_ => 1)
            .Select(g => new
            {
                ValorTotal = g.Sum(x => x.ValorTotal),
                TotalArquivos = g.Select(x => x.IdFilename).Distinct().Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

            //var rendimento = await _dirfRepository.Query()
            //    .Where(d =>
            //        d.IdEmpresa == idEmpresa &&
            //        d.AnoCalendario.ToString() == ano &&
            //        codigos.Contains(d.Codigo)
            //    )
            //    .SumAsync(d => d.ValorRendimento, cancellationToken);


            //var tributo = await _dirfRepository.Query()
            //  .Where(d =>
            //      d.IdEmpresa == idEmpresa &&
            //      d.AnoCalendario.ToString() == ano 
            //  )
            //  .SumAsync(d => d.ValorTributo, cancellationToken);

            //var Dirfs = await _dirfRepository.Query()
            //  .Where(d =>
            //      d.IdEmpresa == idEmpresa &&
            //      d.AnoCalendario.ToString() == ano &&
            //      codigos2.Contains(d.Codigo)
            //  )
            //  .SumAsync(d => d.ValorRendimento, cancellationToken);

            var dirf = await _dirfRepository.Query()
            .Where(d =>
                d.IdEmpresa == idEmpresa &&
                d.AnoCalendario.ToString() == ano
            )
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Rendimento = g
                    .Where(x => codigos.Contains(x.Codigo))
                    .Sum(x => x.ValorRendimento),

                Tributo = g
                    .Sum(x => x.ValorTributo),

                ValorNotas = g
                    .Where(x => codigos2.Contains(x.Codigo))
                    .Sum(x => x.ValorRendimento),
                //ArquivosRendimento = g
                //    .Where(x => codigos.Contains(x.Codigo))
                //    .Select(x => x.IdFilename)
                //    .Distinct()
                //    .Count(),

                TotalArquivos = g
                    .Select(x => x.IdFilename)
                    .Distinct()
                    .Count()
            })
            .FirstOrDefaultAsync(cancellationToken);




            //var pgdasds = await _pgdasdRepository.Query()
            //  .Where(d =>
            //      d.IdEmpresa == idEmpresa &&
            //      d.Periodo.ToString() == ano
            //  )
            //   .SumAsync(d => d.ReceitaBruta, cancellationToken);

            var pgdasds = await _pgdasdRepository.Query()
            .Where(d =>
                d.IdEmpresa == idEmpresa &&
                 d.Periodo.ToString() == ano
            )
            .GroupBy(_ => 1)
            .Select(g => new
            {
                ReceitaBruta = g.Sum(x => x.ReceitaBruta),
                TotalArquivos = g.Select(x => x.IdFilename).Distinct().Count()
            })
            .FirstOrDefaultAsync(cancellationToken);



            return new GetApiResponse
            {
                Data = new
                {
                    v1 = pgdasds?.ReceitaBruta ?? 0,
                    v2 = darfs?.ValorTotal ?? 0,
                    v3 = dirf?.Rendimento ?? 0,
                    v4 = dirf?.Tributo ?? 0,
                    v5 = dirf?.ValorNotas ?? 0,
                    count_files = new
                    {
                        v1 = pgdasds?.TotalArquivos ?? 0,
                        v2 = darfs?.TotalArquivos ?? 0,
                        v3 = dirf?.TotalArquivos ?? 0,
                        v4 = dirf?.TotalArquivos ?? 0,
                        v5 = dirf?.TotalArquivos ?? 0,
                        //v7 = dctf?.TotalArquivos ?? 0
                    }
                }
            };
        }
    }
}
