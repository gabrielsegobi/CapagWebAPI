using Application.Queries.AnalisesICP;
using Domain.Contracts.AnalisesICP;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Application.Handlers.AnalisesICP
{
    public class GetAnaliseIcpRelatorioHandler
        : IRequestHandler<GetAnaliseIcpRelatorioQuery, List<AnaliseIcpRelatorioDto>>
    {
        private readonly IBaseRepository<AnaliseICP> _analiseIcpRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;

        public GetAnaliseIcpRelatorioHandler(
            IBaseRepository<AnaliseICP> analiseIcpRepository,
            IBaseRepository<Empresa> empresaRepository)
        {
            _analiseIcpRepository = analiseIcpRepository;
            _empresaRepository = empresaRepository;
        }

        public async Task<List<AnaliseIcpRelatorioDto>> Handle(
            GetAnaliseIcpRelatorioQuery request,
            CancellationToken cancellationToken)
        {
            var query =
                from ai in _analiseIcpRepository.Query()
                join e in _empresaRepository.Query()
                    on ai.IdEmpresa equals e.IdEmpresa
                select new AnaliseIcpRelatorioDto
                {
                    Cnpj = e.Cnpj,
                    RazaoSocial = e.RazaoSocial,
                    Classificacao = ai.Classificacao,
                    IcpCalculado = ai.IcpCalculado.ToString("N4", new CultureInfo("pt-BR"))
                };

            return await query.ToListAsync(cancellationToken);
        }
    }
}
