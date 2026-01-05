using Application.Queries.DemonstrativosContabeis;
using AutoMapper;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class GetDClByAnoAndCodigoHandler : IRequestHandler<GetDClByAnoAndCodigoQuery, List<DClByAnoAndCodigoDto>>
    {
        private readonly IBaseRepository<DemonstrativoContabil> _baseRepository;
        private readonly IMapper _mapper;

        public GetDClByAnoAndCodigoHandler(IBaseRepository<DemonstrativoContabil> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<List<DClByAnoAndCodigoDto>> Handle(GetDClByAnoAndCodigoQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query(x =>
                x.IdEmpresa == request.IdEmpresa &&
                !string.IsNullOrWhiteSpace(x.Codigo));

            var resultado = new List<DClByAnoAndCodigoDto>();

            if (request.Ano)
            {
                var agrupado = await query
                    .GroupBy(x => new { x.Codigo, x.Ano })
                    .Select(g => new
                    {
                        g.Key.Codigo,
                        g.Key.Ano,
                        TotalValCtaRefFin = g.Sum(x => x.ValCtaRefFin),
                        TotalValCtaRefIni = g.Sum(x => x.ValCtaRefIni)
                    })
                    .ToListAsync(cancellationToken);

                foreach (var item in agrupado)
                {
                    resultado.Add(new DClByAnoAndCodigoDto
                    {
                        Codigo = item.Codigo,
                        Ano = item.Ano,
                        Valor = item.TotalValCtaRefFin ?? 0
                    });

                    if (item.TotalValCtaRefIni != null)
                    {
                        resultado.Add(new DClByAnoAndCodigoDto
                        {
                            Codigo = $"{item.Codigo}[I]",
                            Ano = item.Ano,
                            Valor = item.TotalValCtaRefIni ?? 0
                        });
                    }
                }
            }
            else
            {
                var agrupado = await query
                    .GroupBy(x => x.Codigo)
                    .Select(g => new
                    {
                        Codigo = g.Key,
                        TotalValCtaRefFin = g.Sum(x => x.ValCtaRefFin),
                        TotalValCtaRefIni = g.Sum(x => x.ValCtaRefIni)
                    })
                    .ToListAsync(cancellationToken);

                foreach (var item in agrupado)
                {
                    resultado.Add(new DClByAnoAndCodigoDto
                    {
                        Codigo = item.Codigo,
                        Valor = item.TotalValCtaRefFin ?? 0
                    });

                    if (item.TotalValCtaRefIni != null)
                    {
                        resultado.Add(new DClByAnoAndCodigoDto
                        {
                            Codigo = $"{item.Codigo}[I]",
                            Valor = item.TotalValCtaRefIni ?? 0
                        });
                    }
                }
            }

            return resultado;
        }

    }
}
