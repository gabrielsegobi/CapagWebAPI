using Application.Exceptions.Base;
using Application.Queries.Views.BalancoPatrimonial;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Views;
using Domain.Entities.Views;
using MediatR;

namespace Application.Handlers.Views.BalancoPatrimonial
{
    public class GetBPViewByIdHandler : IRequestHandler<GetBPViewByIdQuery, GetApiResponse>
    {
        private readonly IBaseViewRepository<BalancoPatrimonialVw> _viewRepository;
        private readonly IMapper _mapper;

        public GetBPViewByIdHandler(IBaseViewRepository<BalancoPatrimonialVw> viewRepository, IMapper mapper)
        {
            _viewRepository = viewRepository;
            _mapper = mapper;
        }

        public async Task<GetApiResponse> Handle(GetBPViewByIdQuery request, CancellationToken cancellationToken)
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
