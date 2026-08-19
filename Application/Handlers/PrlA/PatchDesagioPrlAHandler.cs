using Application.Commands.PrlA;
using Application.Exceptions.Empresas;
using Application.Services.PrlA;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.PrlA
{
    public class PatchDesagioPrlAHandler : IRequestHandler<PatchDesagioPrlACommand, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IPrlAService _prlAService;
        private readonly ICurrentUserService _currentUser;

        public PatchDesagioPrlAHandler(
            IBaseRepository<Empresa> empresaRepository,
            IPrlAService prlAService,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _prlAService = prlAService;
            _currentUser = currentUser;
        }

        public async Task<GetApiResponse> Handle(PatchDesagioPrlACommand request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var atualizadoPor = _currentUser.UserId?.ToString() ?? _currentUser.Email ?? "sistema";
            var patch = await _prlAService.AlterarDesagioAsync(
                request.EmpresaId,
                request.CodigoConta,
                request.Request.PercentualDesagio,
                atualizadoPor,
                cancellationToken);

            return new GetApiResponse
            {
                Message = "Deságio atualizado com sucesso",
                Data = patch
            };
        }
    }
}
