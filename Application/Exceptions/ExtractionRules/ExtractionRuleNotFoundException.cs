using Application.Exceptions.Base;

namespace Application.Exceptions.ExtractionRules
{
    public class ExtractionRuleNotFoundException(long id) : NotFoundException($"Regra com o ID: {id} não encontrada.")
    {
    }
}
