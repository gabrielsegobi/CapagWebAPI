using Application.Commands.SimulacoesIntervalo;
using Application.Exceptions.Empresas;
using Application.Exceptions.SimulacoesIntervalo;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.SimulacoesIntervalo
{
    public class CreateSimulacaoIntervaloHandler : IRequestHandler<CreateSimulacaoIntervaloCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<SimulacaoIntervalo> _baseRepository;
        private readonly IMapper _mapper;
        private readonly IBaseRepository<Empresa> _empresaRepository;

        public static string CreateMessage = "Intervalo Criado com sucesso";
        public CreateSimulacaoIntervaloHandler(IBaseRepository<SimulacaoIntervalo> baseRepository, IMapper mapper, IBaseRepository<Empresa> empresaRepository)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
            _empresaRepository = empresaRepository;
        }

        public async Task<CreateApiResponse> Handle(CreateSimulacaoIntervaloCommand request, CancellationToken cancellationToken)
        {
            var intervalo = _mapper.Map<SimulacaoIntervalo>(request.Request)
               ?? throw new InvalidDataException("Invalid data");

            _ = await _empresaRepository.GetByIdAsync(request.Request.IdEmpresa)
                ?? throw new EmpresaNotFoundException(request.Request.IdEmpresa);

            if (request.Request.TipoIntervalo == "ENTRADA")
            {
                var jaExisteEntrada = await _baseRepository.GetFirstOrDefaultAsync(x => x.IdSimulacaoCalc == request.Request.IdSimulacaoCalc && x.TipoIntervalo == "ENTRADA");

                if (jaExisteEntrada != null)
                {
                    throw new DuplicateIntervalException();
                }

            }

            await _baseRepository.AddAsync(intervalo);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse { Message = CreateMessage };
        }
    }
}
