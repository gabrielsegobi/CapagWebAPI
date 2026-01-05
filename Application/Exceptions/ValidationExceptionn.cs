using Application.Exceptions.Base;

namespace Application.Exceptions
{
    public class ValidationException(IEnumerable<string> errors) : AppException("Ocorreram erros de validação.", 400, "VALIDATION_ERROR", errors)
    {
    }
}
