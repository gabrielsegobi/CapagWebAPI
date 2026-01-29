using Application.Commands.SimulacoesCalc;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.SimulacoesCalc
{
    public class CreateSimulacaoCalcHandler : IRequestHandler<CreateSimulacaoCalcCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<SimulacaoCalc> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IMapper _mapper;

        public static string CreateMessage = "Simulação criada com sucesso";
        public CreateSimulacaoCalcHandler(IBaseRepository<SimulacaoCalc> baseRepository, IBaseRepository<Empresa> empresaRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateSimulacaoCalcCommand request, CancellationToken cancellationToken)
        {
            var calc = _mapper.Map<SimulacaoCalc>(request.Request)
                ?? throw new InvalidDataException("Invalid data");

            _ = await _empresaRepository.GetByIdAsync(request.Request.IdEmpresa)
                ?? throw new EmpresaNotFoundException(request.Request.IdEmpresa);

            await _baseRepository.AddAsync(calc);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse { Message = CreateMessage };
        }
    }
}
