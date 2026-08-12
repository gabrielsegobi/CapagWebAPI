using Application.Commands.CodigosRegistroDescricao;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.CodigosRegistroDescricao
{
    public class CreateCodigoRegistroDescricaoHandler : IRequestHandler<CreateCodigoRegistroDescricaoCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<CodigoRegistroDescricao> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IMapper _mapper;

        public CreateCodigoRegistroDescricaoHandler(
            IBaseRepository<CodigoRegistroDescricao> baseRepository,
            IBaseRepository<Empresa> empresaRepository,
            IMapper mapper)
        {
            _baseRepository = baseRepository;
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateCodigoRegistroDescricaoCommand request, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<CodigoRegistroDescricao>(request.Request)
                ?? throw new InvalidDataException("Invalid data");

            _ = await _empresaRepository.GetByIdAsync(request.Request.IdEmpresa)
                ?? throw new EmpresaNotFoundException(request.Request.IdEmpresa);

            await _baseRepository.AddAsync(entity);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse("Código de registro descrição criado com sucesso", entity.Id);
        }
    }
}
