using Application.Queries.UsuariosTenants;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.UsuarioTenant;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.UsuariosTenants
{
    public class GetAllUsuariosTenantsHandler : IRequestHandler<GetAllUsuariosTenantQuery, PagedApiResponse<UsuarioTenantDto>>
    {
        private readonly IBaseRepository<UsuarioTenant> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllUsuariosTenantsHandler (IBaseRepository<UsuarioTenant> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<UsuarioTenantDto>> Handle(GetAllUsuariosTenantQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<UsuarioTenant, UsuarioTenantDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Filter.Papel))
                        q = q.Where(e => e.Papel.Contains(request.Filter.Papel.Trim()));

                    if (request.Filter.IdTenant.HasValue)
                        q = q.Where(e => e.IdTenant == request.Filter.IdTenant.Value);

                    if (request.Filter.IdUsuario.HasValue)
                        q = q.Where(e => e.IdUsuario == request.Filter.IdUsuario.Value);

                    if (request.Filter.Ativo.HasValue)
                        q = q.Where(e => e.Ativo == request.Filter.Ativo.Value);

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Papel)
                        : q.OrderBy(e => e.Papel);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<UsuarioTenantDto>>(data)
            );

            return pagedResult;
        }
    }
}
