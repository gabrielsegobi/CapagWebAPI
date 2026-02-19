namespace Domain.Enums
{
    public enum OperationStatus : short
    {
        Criada = 1,                  
        AguardandoProcessamento = 2, 
        AguardandoProcessamentoComSobrescrita = 3,
        EmProcessamento = 4,          
        Processada = 5,
        ProcessadaComErro = 6,
        Reprocessando = 7,             
        Cancelada = 8                 
    }
}
