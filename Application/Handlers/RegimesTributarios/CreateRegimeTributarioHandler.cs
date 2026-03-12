using Application.Commands.RegimesTributarios;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegimesTributarios
{
    public class CreateRegimeTributarioHandler : IRequestHandler<CreateRegimeTributarioCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegimeTributario> _baseRepository;
        private readonly IMapper _mapper;

        public CreateRegimeTributarioHandler(IBaseRepository<RegimeTributario> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<CreateApiResponse> Handle(CreateRegimeTributarioCommand request, CancellationToken cancellationToken)
        {
            var regime = _mapper.Map<RegimeTributario>(request.CreateRegimeTributarioRequest);
            if (regime == null)
            {
                throw new InvalidDataException("Invalid data");
            }
            await _baseRepository.AddAsync(regime);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse("Regime criado com sucesso.", regime.Id);
        }
    }
}
