using Application.Exceptions.RegDarf;
using Application.Queries.RegDarf;
using AutoMapper;
using Domain.Contracts.RegDarf;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDarf
{
    public class GetRegDarfByIdHandler : IRequestHandler<GetRegDarfByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegDarfs> _baseRepository;
        private readonly IMapper _mapper;
        public GetRegDarfByIdHandler(IBaseRepository<RegDarfs> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetRegDarfByIdQuery request, CancellationToken cancellationToken)
        {
            var darf = await _baseRepository.GetByIdAsync(request.Id) ?? throw new RegDarfNotFoundException(request.Id);

            var response = _mapper.Map<RegDarfDto>(darf);

            return new GetApiResponse { Data = response, Message = "Darf encontrado com sucesso" };
        }
    }
}
