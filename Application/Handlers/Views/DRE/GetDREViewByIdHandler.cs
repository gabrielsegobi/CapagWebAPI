using Application.Exceptions.Base;
using Application.Queries.Views.DRE;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using Domain.Entities.Views;
using MediatR;

namespace Application.Handlers.Views.DRE
{
    public class GetDREViewByIdHandler : IRequestHandler<GetDREViewByIdQuery, GetApiResponse>
    {
        private readonly IBaseViewRepository<DREVw> _viewRepository;
        private readonly IMapper _mapper;

        public GetDREViewByIdHandler(IBaseViewRepository<DREVw> viewRepository, IMapper mapper)
        {
            _viewRepository = viewRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetDREViewByIdQuery request, CancellationToken cancellationToken)
        {
            var view = await _viewRepository.GetByIdAsync(request.Id);

            if (view == null)
                throw new NotFoundException($"Registro do Balanço Patrimonial com ID {request.Id} não encontrado.");

            var dto = _mapper.Map<BPViewDto>(view);

            return new GetApiResponse
            {
                Data = dto,
                Message = "Registro encontrado com sucesso"
            };
        }
    }
}
