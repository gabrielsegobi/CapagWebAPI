using Application.Commands.ResultadosIndicesICP;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ResultadosIndicesICP
{
    public class CreateResultadoIndiceICPHhandler : IRequestHandler<CreateResultadoIndiceICPCommand>
    {
        private readonly IBaseRepository<ResultadoIndiceICP> _baseRepository;
        private readonly IMapper _mapper;

        public CreateResultadoIndiceICPHhandler(IBaseRepository<ResultadoIndiceICP> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task Handle(CreateResultadoIndiceICPCommand request, CancellationToken cancellationToken)
        {

            if (request.CreateResultadosIndicesICPRequest == null || !request.CreateResultadosIndicesICPRequest.Any())
                throw new Exception("Nenhum resultado para salvar.");

            var resultados = _mapper.Map<List<ResultadoIndiceICP>>(request.CreateResultadosIndicesICPRequest);
        
            await _baseRepository.AddRangeAsync(resultados);
            await _baseRepository.SaveChangesAsync();
        }
    }
}