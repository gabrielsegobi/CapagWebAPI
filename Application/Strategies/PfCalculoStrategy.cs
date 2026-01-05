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
            var totalIrpf = await _baseRepository.Query().FirstOrDefaultAsync(irp => irp.IdEmpresa == idEmpresa && irp.AnoCalendario.ToString() == ano)
                ?? throw new RegIrpfNotFoundException(idEmpresa, ano);

            var result = _mapper.Map<PfCalculoDto>(totalIrpf);

            return new GetApiResponse { Data = result, Message = "Parâmetros Calculados com sucesso" };

            //.Where(d =>
            //    d.IdEmpresa == idEmpresa &&
            //    d.AnoCalendario.ToString() == ano
            //)
            //.ToList();
        }
    }
}
