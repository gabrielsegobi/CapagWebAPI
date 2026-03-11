using Application.Exceptions.RegIrpf;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Contracts.TipoGrupos;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Application.Strategies
{
    public class PfCalculoStrategy : ICalculoGrupoStrategy
    {
        public string Tag => "pessoa_fisica";

        private readonly IBaseRepository<RegIrpf> _baseRepository;
        private readonly IMapper _mapper;

        public PfCalculoStrategy(IBaseRepository<RegIrpf> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<GetApiResponse> CalcularAsync(long idEmpresa, string ano, CancellationToken cancellationToken)
        {
            //var totalIrpf = await _baseRepository.Query().FirstOrDefaultAsync(irp => irp.IdEmpresa == idEmpresa && irp.AnoCalendario.ToString() == ano)
            //    ?? throw new RegIrpfNotFoundException(idEmpresa, ano);

            //var result = _mapper.Map<PfCalculoDto>(totalIrpf);



            var irpf = await _baseRepository.Query()
                .Where(d =>
                    d.IdEmpresa == idEmpresa &&
                    d.AnoCalendario.ToString() == ano
                )
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    Registro = g.FirstOrDefault(),
                    TotalArquivos = g.Select(x => x.IdFilename).Distinct().Count()
                })
                .FirstOrDefaultAsync(cancellationToken)
                ?? throw new RegIrpfNotFoundException(idEmpresa, ano);

            var result = _mapper.Map<PfCalculoDto>(irpf.Registro);
            return new GetApiResponse
            {
                Data = new
                {
                    valor_v1 = result.ValorV1,
                    valor_v2 = result.ValorV2,
                    valor_v3 = result.ValorV3,
                    valor_v4 = result.ValorV4,
                    valor_v6 = result.ValorV6,
                    valor_v7 = result.ValorV7,
                    count_files = new
                    {
                        v1 = irpf.TotalArquivos,
                        v2 = irpf.TotalArquivos,
                        v3 = irpf.TotalArquivos,
                        v4 = irpf.TotalArquivos,
                        v6 = irpf.TotalArquivos,
                        v7 = irpf.TotalArquivos
                    }
                },
                Message = "Parâmetros Calculados com sucesso"
            };

            //return new GetApiResponse { Data = result, Message = "Parâmetros Calculados com sucesso" };

            //.Where(d =>
            //    d.IdEmpresa == idEmpresa &&
            //    d.AnoCalendario.ToString() == ano
            //)
            //.ToList();
        }
    }
}
