using Application.Queries.Usuarios;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.Usuarios;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;


namespace Application.Handlers.Usuarios
{
    public class GetAllUsuariosHandler : IRequestHandler<GetAllUsuariosQuery, PagedApiResponse<UsuarioDto>>
    {
        private readonly IBaseRepository<Usuario> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllUsuariosHandler(IBaseRepository<Usuario> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<PagedApiResponse<UsuarioDto>> Handle(GetAllUsuariosQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<Usuario, UsuarioDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Filter.Nome))
                        q = q.Where(e => e.Nome.Contains(request.Filter.Nome.Trim()));

                    if (!string.IsNullOrWhiteSpace(request.Filter.Email))
                        q = q.Where(e => e.Email.Contains(request.Filter.Email.Trim()));

                    q = request.Filter.OrderByDescending
                        ? q.OrderByDescending(e => e.Nome)
                        : q.OrderBy(e => e.Nome);

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<UsuarioDto>>(data)
            );

            return pagedResult;
        }
    }
}
