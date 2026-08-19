using Application.Commands.PrlA;
using Application.Exceptions.Empresas;
using Application.Services.PrlA;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.PrlA
{
    public class PatchBlocoPrlAHandler : IRequestHandler<PatchBlocoPrlACommand, GetApiResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IPrlAService _prlAService;
        private readonly ICurrentUserService _currentUser;

        public PatchBlocoPrlAHandler(
            IBaseRepository<Empresa> empresaRepository,
            IPrlAService prlAService,
            ICurrentUserService currentUser)
        {
            _empresaRepository = empresaRepository;
            _prlAService = prlAService;
            _currentUser = currentUser;
        }

        public async Task<GetApiResponse> Handle(PatchBlocoPrlACommand request, CancellationToken cancellationToken)
        {
            var existe = await _empresaRepository.AnyAsync(e => e.IdEmpresa == request.EmpresaId && e.DeletedAt == null);
            if (!existe)
                throw new EmpresaNotFoundException(request.EmpresaId);

            var atualizadoPor = _currentUser.UserId?.ToString() ?? _currentUser.Email ?? "sistema";
            var patch = await _prlAService.AlterarBlocoAsync(
                request.EmpresaId,
                request.CodigoConta,
                request.Request.Bloco,
                atualizadoPor,
                cancellationToken);

            return new GetApiResponse
            {
                Message = "Bloco de liquidez atualizado com sucesso",
                Data = patch
            };
        }
    }
}
