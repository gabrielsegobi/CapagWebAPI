using Application.Queries.Empresas;
using AutoMapper;
using Domain.Contracts.Empresas;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.Empresas
{
    public class GetAllEmpresasHandler : IRequestHandler<GetAllEmpresasQuery, PagedApiResponse<EmpresaDto>>
    {
        private readonly IBaseRepository<Empresa> _baseRepository;
        private readonly IMapper _mapper;

        public GetAllEmpresasHandler(IBaseRepository<Empresa> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<EmpresaDto>> Handle(GetAllEmpresasQuery request, CancellationToken cancellationToken)
        {
            var query = _baseRepository.Query();

            var pagedResult = await query.ReadPage<Empresa, EmpresaDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (!string.IsNullOrWhiteSpace(request.Filter.Cnpj))
                        q = q.Where(e => e.Cnpj.ToString().Contains(request.Filter.Cnpj.Trim()));

                    var sort = request.Filter.Sort?.Trim().ToLower();

                    q = sort switch
                    {
                        "razao_social" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.RazaoSocial)
                            : q.OrderBy(e => e.RazaoSocial),

                        "cnpj" => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Cnpj)
                            : q.OrderBy(e => e.Cnpj),

                        _ => request.Filter.OrderByDescending
                            ? q.OrderByDescending(e => e.Cnpj)
                            : q.OrderBy(e => e.Cnpj)
                    };

                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<EmpresaDto>>(data)
            );

            return pagedResult;
        }
    }
}
