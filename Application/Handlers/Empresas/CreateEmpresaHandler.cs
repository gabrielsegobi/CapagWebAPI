using Application.Commands.Empresas;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Empresas
{
    public class CreateEmpresaHandler : IRequestHandler<CreateEmpresaCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<Empresa> _baseRepository;
        private readonly IMapper _mapper;

        public CreateEmpresaHandler(IBaseRepository<Empresa> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateEmpresaCommand request, CancellationToken cancellationToken)
        {
            var empresa = _mapper.Map<Empresa>(request.CreateEmpresaRequest);
            if (empresa == null)
                throw new InvalidDataException("Invalid data");

            var empresaExistente = await _baseRepository.GetFirstOrDefaultAsync(e => e.Cnpj == empresa.Cnpj);

           if (empresaExistente != null)
                throw new EmpresaCnpjConflictException(empresa.Cnpj);

            await _baseRepository.AddAsync(empresa);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Empresa Criada com Sucesso"
            };
        }
    }
}
