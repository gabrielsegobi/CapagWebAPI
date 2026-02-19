using Domain.Enums;

namespace Domain.Entities
{
    public class OperationFile
    {
        public OperationFile(long idOp, string fileName, OperationFileStatus status)
        {
            IdOp = idOp;
            FileName = fileName;
            Status = status;
        }

        public long Id { get; set; }

        // ========= Operation =========
        public long IdOp { get; set; }
        public Operation Operation { get; set; } = null!;

        public string FileName { get; set; } = string.Empty;

        public OperationFileStatus Status { get; set; }


        public void Start()
        {
            if (Status != OperationFileStatus.AguardandoProcessamento)
                throw new Exception($"Operação não pode iniciar no status '{Status}'.");

            Status = OperationFileStatus.EmProcessamento;
        }
        public void Reprocessing()
        {
            if (Status != OperationFileStatus.Processado && Status != OperationFileStatus.ProcessadaComErro && Status != OperationFileStatus.Cancelado)
                throw new Exception($"Operação não pode aguardar processamento no status '{Status}'.");

            Status = OperationFileStatus.Reprocessando;
        }

        public void Finish()
        {
            if (Status != OperationFileStatus.EmProcessamento && Status != OperationFileStatus.Reprocessando)
                throw new Exception($"Operação não pode iniciar no status '{Status}'.");

            Status = OperationFileStatus.Processado;
        }
        public void Error()
        {
            Status = OperationFileStatus.ProcessadaComErro;
        }

    }
}
