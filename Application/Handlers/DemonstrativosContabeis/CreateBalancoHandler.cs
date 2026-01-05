using Application.Commands.DemonstrativosContabeis;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class CreateBalancoHandler : IRequestHandler<CreateBalancoCommand>
    {
        private readonly IBaseRepository<DemonstrativoContabil> _baseRepository;
        private readonly IMapper _mapper;

        public CreateBalancoHandler(IBaseRepository<DemonstrativoContabil> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task Handle(CreateBalancoCommand request, CancellationToken cancellationToken)
        {
            if (request.CreateBalancoRequests == null || !request.CreateBalancoRequests.Any())
                throw new InvalidDataException("Nenhum demonstrativo contábil foi informado.");

            var balancos = _mapper.Map<List<DemonstrativoContabil>>(request.CreateBalancoRequests);


            if (balancos == null || !balancos.Any())
                throw new InvalidDataException("Falha ao mapear dados para DemonstrativoContabil.");

            await _baseRepository.AddRangeAsync(balancos);
            await _baseRepository.SaveChangesAsync();
        }
    }
}
