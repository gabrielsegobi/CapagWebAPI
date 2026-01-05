using Application.Commands.Empresas;
using Application.Exceptions.Empresas;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Empresas
{
    public class UpdateEmpresaHandler : IRequestHandler<UpdateEmpresaCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<Empresa> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateEmpresaHandler(IBaseRepository<Empresa> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<UpdateApiResponse> Handle(UpdateEmpresaCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _baseRepository.GetByIdAsync(request.EmpresaId);
            if (empresa == null)
            {
                throw new EmpresaNotFoundException(request.EmpresaId);
            }

            var empresaExistente = await _baseRepository.GetFirstOrDefaultAsync(e => e.Cnpj == request.UpdateEmpresaRequest.Cnpj && e.IdEmpresa != request.EmpresaId);

            if (empresaExistente != null)
                throw new EmpresaCnpjConflictException(empresa.Cnpj);

            var EmpresaToUpdate = _mapper.Map(request.UpdateEmpresaRequest, empresa);

            _baseRepository.Update(EmpresaToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Empresa atualizada com sucesso." };
        }
    }
}
