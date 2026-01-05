using Application.Commands.AnalisesICP;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.AnalisesICP
{
    public class CreateAnaliseICPHandler : IRequestHandler<CreateAnaliseICPCommand>
    {
        private readonly IBaseRepository<AnaliseICP> _baseRepository;
        private readonly IMapper _mapper;

        public CreateAnaliseICPHandler(IBaseRepository<AnaliseICP> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task Handle(CreateAnaliseICPCommand request, CancellationToken cancellationToken)
        {
            var analise = _mapper.Map<AnaliseICP>(request.CreateAnaliseICPRequest);

            if (analise == null)
            {
                throw new InvalidDataException("Invalid data");
            }

            await _baseRepository.AddAsync(analise);
            await _baseRepository.SaveChangesAsync();
        }
    }
}
