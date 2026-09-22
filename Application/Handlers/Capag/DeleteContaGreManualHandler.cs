using Application.Commands.Capag;
using Application.Exceptions.Capag;
using Application.Exceptions.Empresas;
using Application.Services.Capag;
using Domain.Contracts.Capag;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Capag
{
    public class DeleteContaGreManualHandler : IRequestHandler<DeleteContaGreManualCommand, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<ContaGreManual> _manualRepository;
        private readonly GreBuilderService _greBuilder;

        public DeleteContaGreManualHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<ContaGreManual> manualRepository,
            GreBuilderService greBuilder)
        {
            _empresaRepository = empresaRepository;
            _manualRepository = manualRepository;
            _greBuilder = greBuilder;
        }

        public async Task<GetApiResponse> Handle(DeleteContaGreManualCommand request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var entity = await _manualRepository.GetFirstOrDefaultAsync(x =>
                x.Id == request.Id && x.EmpresaId == request.EmpresaId)
                ?? throw new ContaGreManualNotFoundException(request.Id);

            _manualRepository.Delete(entity);
            await _manualRepository.SaveChangesAsync();

            var gre = await _greBuilder.ConstruirGre(request.EmpresaId, cancellationToken);

            return new GetApiResponse
            {
                Message = "Conta manual removida com sucesso",
                Data = new ContaGreManualResponse
                {
                    IdEmpresa = request.EmpresaId,
                    Conta = null,
                    Gre = gre
                }
            };
        }
    }
}
