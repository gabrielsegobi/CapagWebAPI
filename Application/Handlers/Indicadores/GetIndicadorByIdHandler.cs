using Application.Exceptions.Indicadores;
using Application.Helpers;
using Application.Queries.Indicadores;
using AutoMapper;
using Domain.Contracts.Indicadores;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.Indicadores
{
    public class GetIndicadorByIdHandler : IRequestHandler<GetIndicadorByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<Indicador> _baseRepository;
        private readonly IMapper _mapper;
        public GetIndicadorByIdHandler(IBaseRepository<Indicador> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetIndicadorByIdQuery request, CancellationToken cancellationToken)
        {
            var indicador = await _baseRepository
                .Query()
                .Include(i => i.ValoresAnuais)
                .FirstOrDefaultAsync(i => i.IdIndicador == request.Id, cancellationToken);

            if (indicador == null || indicador.DeletedAt != null)
            {
                throw new IndicadorNotFoundException(request.Id);
            }

            var result = _mapper.Map<IndicadorDto>(indicador);
            var formulas = await IndicadoresFormulaHelper.CarregarFormulasAsync(cancellationToken);
            IndicadoresFormulaHelper.EnriquecerIndicadores([result], formulas);

            return new GetApiResponse
            {
                Data = result,
                Message = "Indicador encontrado com sucesso"
            };
        }
    }
}
