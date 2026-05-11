using Application.Commands.Operations;
using Application.Commands.SPED.ECF;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Infrastructure.BackgroundJobs;
using Infrastructure.Interface;
using MediatR;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Application.Handlers.SPED.ECF
{
    public class UploadECFHandler : IRequestHandler<UploadECFCommand>
    {
        private readonly IObjectStorage _objectStorage;
        private readonly IMediator _mediator;
        private readonly IBaseRepository<Operation> _operationRepository;
        private readonly IBaseRepository<OperationFile> _operationFileRepository;
        private readonly EcfBackgroundWorker _worker;

        public UploadECFHandler(IObjectStorage objectStorage, IMediator mediator, IBaseRepository<Operation> operationRepository, IBaseRepository<OperationFile> operationFileRepository, EcfBackgroundWorker worker)
        {
            _objectStorage = objectStorage;
            _mediator = mediator;
            _operationRepository = operationRepository;
            _operationFileRepository = operationFileRepository;
            _worker = worker;
        }

        public async Task Handle(UploadECFCommand request, CancellationToken cancellationToken)
        {
            var idEmpresa = request.Request.IdEmpresa;

            var idOp = await _mediator.Send(new CreateOperationCommand(idEmpresa), cancellationToken);

            var folderKey = idOp.ToString();

            foreach (var file in request.Request.Files)
            {

                await using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream, leaveOpen: true);

                // 3️⃣ lê a primeira linha
                var primeiraLinha = await reader.ReadLineAsync(cancellationToken);

                if (string.IsNullOrWhiteSpace(primeiraLinha))
                    continue;

                var competencia = ExtrairCompetencia(primeiraLinha);
                var cnpjRaiz = ExtrairCnpjRaiz(primeiraLinha);

                // volta o stream para salvar o arquivo inteiro
                stream.Position = 0;
                var uniqueName = $"{Guid.NewGuid()}_{file.FileName}";
                // 4️⃣ salva o arquivo dentro da pasta da operação
                await _objectStorage.SaveAsync(
                    folderKey,
                    stream,
                   uniqueName
                );

                var operationFile = new OperationFile(idOp, uniqueName, OperationFileStatus.AguardandoProcessamento);
                await _operationFileRepository.AddAsync(operationFile);
            }
            await _operationFileRepository.SaveChangesAsync();

            var operation = await _operationRepository.GetByIdAsync(idOp) ?? throw new Exception();

            if (request.Request.Overwrite)
            {
                operation.AwaitProcessingWithOverwrite();
            }
            else
            {
                operation.AwaitProcessing();
            }
            _operationRepository.Update(operation);
            await _operationRepository.SaveChangesAsync();
            _worker.Trigger();
        }

        private static string ExtrairCompetencia(string linha)
        {
            var data = ExtrairDataInicial(linha);

            if (DateTime.TryParseExact(
                data,
                "ddMMyyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dt))
            {
                return dt.ToString("yyyyMM");
            }

            return string.Empty;
        }

        private static string ExtrairDataInicial(string linha)
        {
            // Campo 10 → data inicial (ddMMyyyy)
            var match = Regex.Match(linha, @"^\|0000\|(?:[^|]*\|){9}(\d{8})\|");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }



        private static string ExtrairCnpjRaiz(string linha)
        {
            var campos = linha.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (campos.Length > 3 && campos[0] == "0000")
                return campos[3].Substring(0, 8);

            return string.Empty;
        }




    }
}
