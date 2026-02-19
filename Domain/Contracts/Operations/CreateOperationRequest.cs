using Domain.Entities;

namespace Domain.Contracts.Operations
{
    public class CreateOperationRequest
    {

        public string OperationName { get; set; } = string.Empty;

        public short Status { get; set; }

        public string Cnpj { get; set; } = string.Empty;

        public long IdEmpresa { get; set; }
    }
}
