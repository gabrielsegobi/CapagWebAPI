using Application.Commands.Empresas;
using Application.Exceptions.Empresas;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Empresas
{
    public class UpdateEmpresaImpedimentoHandler : IRequestHandler<UpdateEmpresaImpedimentoCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<Empresa> _baseRepository;

        public UpdateEmpresaImpedimentoHandler(IBaseRepository<Empresa> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public async Task<UpdateApiResponse> Handle(UpdateEmpresaImpedimentoCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _baseRepository.GetByIdAsync(request.EmpresaId);
            if (empresa == null)
                throw new EmpresaNotFoundException(request.EmpresaId);

            empresa.DataImpedimento = request.UpdateEmpresaImpedimentoRequest.DataImpedimento;
            empresa.IdUsuarioResponsavel = request.UpdateEmpresaImpedimentoRequest.IdUsuarioResponsavel;
            empresa.UpdatedAt = DateTimeHelper.GetDateTimeNow();

            _baseRepository.Update(empresa);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Impedimento da empresa atualizado com sucesso." };
        }
    }
}
