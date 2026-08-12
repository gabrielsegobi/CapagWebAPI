using Application.Exceptions.CodigosRegistroDescricao;
using Application.Queries.CodigosRegistroDescricao;
using AutoMapper;
using Domain.Contracts.CodigosRegistroDescricao;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CodigosRegistroDescricao
{
    public class GetCodigoRegistroDescricaoByIdHandler : IRequestHandler<GetCodigoRegistroDescricaoByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<CodigoRegistroDescricao> _baseRepository;
        private readonly IMapper _mapper;

        public GetCodigoRegistroDescricaoByIdHandler(IBaseRepository<CodigoRegistroDescricao> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<GetApiResponse> Handle(GetCodigoRegistroDescricaoByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new CodigoRegistroDescricaoNotFoundException(request.Id);

            return new GetApiResponse
            {
                Data = _mapper.Map<CodigoRegistroDescricaoDto>(entity),
                Message = "Código de registro descrição encontrado com sucesso"
            };
        }
    }
}
