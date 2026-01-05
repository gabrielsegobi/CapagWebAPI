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

            var Dirfs = await _dirfRepository.Query()
              .Where(d =>
                  d.IdEmpresa == idEmpresa &&
                  d.AnoCalendario.ToString() == ano &&
                  codigos2.Contains(d.Codigo)
              )
              .SumAsync(d => d.ValorRendimento, cancellationToken);

            var pgdasds = await _pgdasdRepository.Query()
              .Where(d =>
                  d.IdEmpresa == idEmpresa &&
                  d.Periodo.ToString() == ano
              )
               .SumAsync(d => d.ReceitaBruta, cancellationToken);




            return new GetApiResponse
            {
                Data = new
                {
                    v1 = pgdasds,
                    v2 = darfs,
                    v3 = rendimento,
                    v4 = tributo,
                    v5 = Dirfs,
                }
            };
        }
    }
}
