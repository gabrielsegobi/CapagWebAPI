using Application.Commands.ValoresAnuais;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ValoresAnuais
{
    public class CreateValorAnualHandler : IRequestHandler<CreateValorAnulCommand>
    {
        private readonly IBaseRepository<ValorAnual> _baseRepository;
        private readonly IMapper _mapper;

        public CreateValorAnualHandler(IBaseRepository<ValorAnual> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task Handle(CreateValorAnulCommand request, CancellationToken cancellationToken)
        {
            var valoranual = _mapper.Map<ValorAnual>(request.CreateValorAnualRequest);

            if (valoranual == null)
            {
                throw new InvalidDataException("Invalid data");
            }

            await _baseRepository.AddAsync(valoranual);
            await _baseRepository.SaveChangesAsync();
        }
    }
}
