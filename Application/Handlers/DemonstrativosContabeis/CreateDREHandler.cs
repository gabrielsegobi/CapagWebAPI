using Application.Commands.DemonstrativosContabeis;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class CreateDREHandler : IRequestHandler<CreateDRECommand>
    {
        private readonly IBaseRepository<DemonstrativoContabil> _baseRepository;
        private readonly IMapper _mapper;

        public CreateDREHandler(IBaseRepository<DemonstrativoContabil> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task Handle(CreateDRECommand request, CancellationToken cancellationToken)
        {
            if (request.CreateDRERequest == null || !request.CreateDRERequest.Any())
                throw new InvalidDataException("Nenhum demonstrativo contábil foi informado.");

            var DREs = _mapper.Map<List<DemonstrativoContabil>>(request.CreateDRERequest);


            if (DREs == null || !DREs.Any())
                throw new InvalidDataException("Falha ao mapear dados para DemonstrativoContabil.");

            await _baseRepository.AddRangeAsync(DREs);
            await _baseRepository.SaveChangesAsync();
        }
    }
}
