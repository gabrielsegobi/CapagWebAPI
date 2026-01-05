using Application.Commands.Indicadores;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Indicadores
{
    public class CreateIndicadorHandler : IRequestHandler<CreateIndicadorCommand>
    {
        private readonly IBaseRepository<Indicador> _baseRepository;
        private readonly IMapper _mapper;

        public CreateIndicadorHandler(IBaseRepository<Indicador> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task Handle(CreateIndicadorCommand request, CancellationToken cancellationToken)
        {
            var indicador = _mapper.Map<Indicador>(request.CreateIndicadorRequest);

            if (indicador == null)
            {
                throw new InvalidDataException("Invalid data");
            }

            await _baseRepository.AddAsync(indicador);
            await _baseRepository.SaveChangesAsync();

        }
    }
}
