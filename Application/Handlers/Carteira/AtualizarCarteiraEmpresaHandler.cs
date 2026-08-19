using Application.Commands.Carteira;
using Application.Exceptions.Empresas;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Carteira
{
    public class AtualizarCarteiraEmpresaHandler : IRequestHandler<AtualizarCarteiraEmpresaCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;

        public AtualizarCarteiraEmpresaHandler(IBaseRepository<Empresa> empresaRepository)
        {
            _empresaRepository = empresaRepository;
        }

        public async Task<UpdateApiResponse> Handle(AtualizarCarteiraEmpresaCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.EmpresaId)
                ?? throw new EmpresaNotFoundException(request.EmpresaId);

            empresa.Status = request.Request.Status;
            empresa.ValorContrato = request.Request.ValorContrato;
            empresa.DataImpedimento = request.Request.DataImpedimento;
            empresa.UpdatedAt = DateTimeHelper.GetDateTimeNow();

            _empresaRepository.Update(empresa);
            await _empresaRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Dados comerciais da empresa atualizados com sucesso." };
        }
    }
}
