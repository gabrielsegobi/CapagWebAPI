namespace Domain.Enums
{
    public enum OperationFileStatus : short
    {
        Recebido = 1,
        AguardandoProcessamento = 2,
        EmProcessamento = 3,
        Processado = 4,
        Reprocessando = 5,
        ProcessadaComErro = 6,
        Cancelado = 7
    }
}
