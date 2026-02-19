using Application.Commands.SPED.ECF;
using Application.Exceptions.Empresas;
using Application.Exceptions.Operations;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.BackgroundJobs;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.SPED.ECF
{
    public class ReprocessEcfHandler : IRequestHandler<ReprocessEcfCommand>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<Operation> _operationRepository;
        private readonly EcfReprocessWorker _worker;

        public ReprocessEcfHandler(IBaseRepository<Empresa> empresaRepository, IBaseRepository<Operation> operationRepository, EcfReprocessWorker worker)
        {
            _empresaRepository = empresaRepository;
            _operationRepository = operationRepository;
            _worker = worker;
        }

        public async Task Handle(ReprocessEcfCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.IdEmpresa) ??
                throw new EmpresaNotFoundException(request.IdEmpresa);



            if (!_operationRepository.Query(o =>
                    o.IdEmpresa == request.IdEmpresa &&
                    o.Status == OperationStatus.ProcessadaComErro &&
                    o.Files.Any(f => f.Status == OperationFileStatus.ProcessadaComErro)
                ).Any())
                throw new OperationWithErrorNotFoundException(request.IdEmpresa);


            _worker.Trigger(request.IdEmpresa);
        }
    }
}
