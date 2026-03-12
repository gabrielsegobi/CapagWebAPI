using Application.Commands.ProcessLog;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ProcessLogs
{
    public class CreateProcessLogHandler : IRequestHandler<CreateProcessLogCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<ProcessLog> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IMapper _mapper;

        public CreateProcessLogHandler(IBaseRepository<ProcessLog> baseRepository, IMapper mapper, IBaseRepository<Empresa> empresaRepository)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
            _empresaRepository = empresaRepository;
        }

        public async Task<CreateApiResponse> Handle(CreateProcessLogCommand request, CancellationToken cancellationToken)
        {
            var log = _mapper.Map<ProcessLog>(request.CreateProcessLogRequest);

            if (log == null)
                throw new InvalidDataException("Invalid data");

            var empresa = await _empresaRepository.GetByIdAsync(request.CreateProcessLogRequest.IdEmpresa);
            if (empresa == null)
            {
                throw new EmpresaNotFoundException(request.CreateProcessLogRequest.IdEmpresa);
            }

            await _baseRepository.AddAsync(log);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse("log de processo criado com sucesso", log.Id);
        }
    }
}
