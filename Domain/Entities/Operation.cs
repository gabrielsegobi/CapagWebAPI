using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Operation
    {
        public Operation(string operationName, long idUsuario, long idEmpresa, long idTenant, string cnpj)
        {
            DateCreate = GetDateTimeNow();
            OperationName = operationName;
            Status = OperationStatus.Criada;
            IdUsuario = idUsuario;
            IdEmpresa = idEmpresa;
            IdTenant = idTenant;
            Cnpj = cnpj;
            Files = [];
        }

        public long Id { get; private set; }

        public DateTime DateCreate { get; private set; }
        public DateTime? DateStart { get; private set; }
        public DateTime? DateFinish { get; private set; }

        public string OperationName { get; private set; } = string.Empty;

        public OperationStatus Status { get; private set; }

        public long IdUsuario { get; private set; }

        public string Cnpj { get; private set; } = string.Empty;

        // ========= Tenant =========
        public long IdTenant { get; set; }
        public Tenant Tenant { get; private set; } = null!;

        // ========= Empresa =========
        public long IdEmpresa { get; private set; }
        public Empresa Empresa { get; private set; } = null!;

        // ========= Arquivos da operação =========
        public ICollection<OperationFile> Files { get; private set; } = new List<OperationFile>();


        private DateTime GetDateTimeNow()
        {
            var now = DateTime.UtcNow;
            return now.AddHours(-3);
        }

        public void Start()
        {
            if (Status != OperationStatus.AguardandoProcessamento && Status != OperationStatus.AguardandoProcessamentoComSobrescrita)
                throw new Exception($"Operação não pode iniciar no status '{Status}'.");

            DateStart = GetDateTimeNow();
            Status = OperationStatus.EmProcessamento;
        }

        public void AwaitProcessing()
        {
            if (Status != OperationStatus.Criada)
                throw new Exception($"Operação não pode aguardar processamento no status '{Status}'.");

            Status = OperationStatus.AguardandoProcessamento;
        }

        public void AwaitProcessingWithOverwrite()
        {
            if (Status != OperationStatus.Criada)
                throw new Exception($"Operação não pode aguardar processamento no status '{Status}'.");

            Status = OperationStatus.AguardandoProcessamentoComSobrescrita;
        }

        public void Reprocessing()
        {
            if (Status != OperationStatus.Processada && Status != OperationStatus.ProcessadaComErro && Status != OperationStatus.Cancelada)
                throw new Exception($"Operação não pode aguardar processamento no status '{Status}'.");

            Status = OperationStatus.Reprocessando;
        }

        public void Error()
        {
            DateFinish = GetDateTimeNow();
            Status = OperationStatus.ProcessadaComErro;
        }

        public void Finish()
        {
            if (Status != OperationStatus.EmProcessamento && Status != OperationStatus.Reprocessando)
                throw new Exception($"Operação não pode ser finalizada no status '{Status}'.");

            DateFinish = GetDateTimeNow();
            Status = OperationStatus.Processada;
        }
    }

}
