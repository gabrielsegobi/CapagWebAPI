using Domain.Contracts.Responses;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

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

            var totalDctf = await _dctfRepository.Query()
                .Where(d =>
                    d.IdEmpresa == idEmpresa
                )
                .SumAsync(d => d.Valor, cancellationToken);

            var dre = await _dreRepository.Query()
                .FirstOrDefaultAsync(d =>
                    d.IdEmpresa == idEmpresa &&
                    d.Ano.ToString() == ano &&
                    d.Codigo == codigoDre
                );


            return new GetApiResponse
            {
                Data = new
                {
                    v1 = darfs,
                    v2 = rendimento,
                    v3 = tributo,
                    v6 = dre?.ValCtaRefFin ?? 0,
                    v7 = totalDctf
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
