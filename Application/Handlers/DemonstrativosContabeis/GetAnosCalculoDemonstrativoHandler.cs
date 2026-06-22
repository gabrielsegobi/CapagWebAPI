using Application.Helpers;
using Application.Queries.DemonstrativosContabeis;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class GetAnosCalculoDemonstrativoHandler : IRequestHandler<GetAnosCalculoDemonstrativoQuery, IReadOnlyList<int>>
    {
        private readonly IBaseRepository<DemonstrativoContabil> _baseRepository;

        public GetAnosCalculoDemonstrativoHandler(IBaseRepository<DemonstrativoContabil> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<IReadOnlyList<int>> Handle(GetAnosCalculoDemonstrativoQuery request, CancellationToken cancellationToken)
        {
            var anosDisponiveis = await _baseRepository
                .Query(x => x.IdEmpresa == request.IdEmpresa)
                .Select(x => x.Ano)
                .Distinct()
                .ToListAsync(cancellationToken);

            return DemonstrativosAnosHelper.ObterJanelaUltimosAnos(anosDisponiveis);
        }
    }
}
