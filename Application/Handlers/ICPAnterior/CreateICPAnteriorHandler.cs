using Application.Commands.ICPAnterior;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ICPAnterior
{
    public class CreateICPAnteriorHandler : IRequestHandler<CreateICPAnteriorCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<ICPsAnterior> _baseRepository;
        private readonly IMapper _mapper;

        public CreateICPAnteriorHandler(IBaseRepository<ICPsAnterior> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateICPAnteriorCommand request, CancellationToken cancellationToken)
        {
            var icpAnterior = _mapper.Map<ICPsAnterior>(request.Request) ?? throw new InvalidDataException("Invalid data");

            await _baseRepository.AddAsync(icpAnterior);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse("ICP Criado com Sucesso", icpAnterior.IdIcpAnterior);
        }
    }
}
