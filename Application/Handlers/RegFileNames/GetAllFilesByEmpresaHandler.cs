using Application.Exceptions.Empresas;
using Application.Queries.RegFileNames;
using Domain.Contracts.RegFileNames;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.RegFileNames
{
    public class GetAllFilesByEmpresaHandler : IRequestHandler<GetAllFilesByEmpresaQuery, GetApiResponse>
    {
        private readonly IBaseRepository<RegDefi> _regDefiRepository;
        private readonly IBaseRepository<RegDarfs> _regDarfsRepository;
        private readonly IBaseRepository<RegDctf> _regDctfRepository;
        private readonly IBaseRepository<RegDirfTerceiro> _regDirfTerceiroRepository;
        private readonly IBaseRepository<RegIrpf> _regIrpfRepository;
        private readonly IBaseRepository<RegPgdasd> _regPgdasdRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;

        public GetAllFilesByEmpresaHandler(IBaseRepository<RegDefi> regDefiRepository, IBaseRepository<RegDarfs> regDarfsRepository,
            IBaseRepository<RegDctf> regDctfRepository, IBaseRepository<RegDirfTerceiro> regDirfTerceiroRepository,
            IBaseRepository<RegIrpf> regIrpfRepository, IBaseRepository<RegPgdasd> regPgdasdRepository, IBaseRepository<Empresa> empresaRepository)
        {
            _regDefiRepository = regDefiRepository;
            _regDarfsRepository = regDarfsRepository;
            _regDctfRepository = regDctfRepository;
            _regDirfTerceiroRepository = regDirfTerceiroRepository;
            _regIrpfRepository = regIrpfRepository;
            _regPgdasdRepository = regPgdasdRepository;
            _empresaRepository = empresaRepository;
        }

        public async Task<GetApiResponse> Handle(GetAllFilesByEmpresaQuery request, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.IdEmpresa) ?? throw new EmpresaNotFoundException(request.IdEmpresa);

            var dctf = _regDctfRepository
            .Query()
            .Where(x => x.IdEmpresa == empresa.IdEmpresa)
            .Select(x => new RegFileNameDto
            {
                Id = x.FileName.Id,
                FileName = x.FileName.FileName,
                Status = x.FileName.Status,
                Error = x.FileName.Error,
                CreatedAt = x.FileName.CreatedAt,
                Type = "DCTF"
            });

            var defi = _regDefiRepository
             .Query()
             .Where(x => x.IdEmpresa == empresa.IdEmpresa)
             .Select(x => new RegFileNameDto
             {
                 Id = x.FileName.Id,
                 FileName = x.FileName.FileName,
                 Status = x.FileName.Status,
                 Error = x.FileName.Error,
                 CreatedAt = x.FileName.CreatedAt,
                 Type = "DEFIS"
             });


            var darf = _regDarfsRepository
             .Query()
             .Where(x => x.IdEmpresa == empresa.IdEmpresa)
             .Select(x => new RegFileNameDto
             {
                 Id = x.FileName.Id,
                 FileName = x.FileName.FileName,
                 Status = x.FileName.Status,
                 Error = x.FileName.Error,
                 CreatedAt = x.FileName.CreatedAt,
                 Type = "DARF"
             });

            var dirf = _regDirfTerceiroRepository
              .Query()
              .Where(x => x.IdEmpresa == empresa.IdEmpresa)
              .Select(x => new RegFileNameDto
              {
                  Id = x.FileName.Id,
                  FileName = x.FileName.FileName,
                  Status = x.FileName.Status,
                  Error = x.FileName.Error,
                  CreatedAt = x.FileName.CreatedAt,
                  Type = "DIRF"
              });

            var irpf = _regIrpfRepository
              .Query()
              .Where(x => x.IdEmpresa == empresa.IdEmpresa)
              .Select(x => new RegFileNameDto
              {
                  Id = x.FileName.Id,
                  FileName = x.FileName.FileName,
                  Status = x.FileName.Status,
                  Error = x.FileName.Error,
                  CreatedAt = x.FileName.CreatedAt,
                  Type = "IRPF"
              });

            var pgdas = _regPgdasdRepository
               .Query()
               .Where(x => x.IdEmpresa == empresa.IdEmpresa)
               .Select(x => new RegFileNameDto
               {
                   Id = x.FileName.Id,
                   FileName = x.FileName.FileName,
                   Status = x.FileName.Status,
                   Error = x.FileName.Error,
                   CreatedAt = x.FileName.CreatedAt,
                   Type = "PGDASD"
               });

            var result = await dctf
                  .Union(darf)
                  .Union(defi)
                  .Union(dirf)
                  .Union(irpf)
                  .Union(pgdas)
                  .OrderByDescending(x => x.CreatedAt)
                  .ToListAsync(cancellationToken);

            return new GetApiResponse { Data = result };


        }
    }
}
