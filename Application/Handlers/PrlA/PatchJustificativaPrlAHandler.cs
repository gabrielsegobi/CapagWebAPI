using Application.Commands.PrlA;
using Application.Exceptions.Empresas;
using Application.Services.PrlA;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.PrlA
{
    public class PatchJustificativaPrlAHandler : IRequestHandler<PatchJustificativaPrlACommand, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IPrlAService _prlAService;
        private readonly ICurrentUserService _currentUser;

        public PatchJustificativaPrlAHandler(
            IBaseRepository<Empresa> empresaRepository,
            IPrlAService prlAService,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _prlAService = prlAService;
            _currentUser = currentUser;
        }

        public async Task<GetApiResponse> Handle(PatchJustificativaPrlACommand request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var atualizadoPor = _currentUser.UserId?.ToString() ?? _currentUser.Email ?? "sistema";
            var patch = await _prlAService.AlterarJustificativaAsync(
                request.EmpresaId,
                request.CodigoConta,
                request.Request.Justificativa,
                atualizadoPor,
                cancellationToken);

            return new GetApiResponse
            {
                Message = "Justificativa atualizada com sucesso",
                Data = patch
            };
        }
    }
}
