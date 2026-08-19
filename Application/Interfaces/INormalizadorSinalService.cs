using Domain.Models;

namespace Application.Interfaces
{
    public interface INormalizadorSinalService
    {
        decimal Normalizar(ContaContabil conta);

        decimal Normalizar(string? codigo, decimal? saldoBruto, char? indicador);
    }
}

