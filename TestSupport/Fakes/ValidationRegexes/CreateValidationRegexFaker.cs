using Bogus;
using Domain.Contracts.ValidatioRegexes;

namespace TestSupport.Fakes.ValidationRegexes
{
    public class CreateValidationRegexRequestFaker : Faker<CreateValidationRegexRequest>
    {
        public CreateValidationRegexRequestFaker()
        {
            RuleFor(x => x.Regex, f => f.PickRandom(
                @"\d{2}\/\d{2}\/\d{4}",                 // Data
                @"\d{3}\.\d{3}\.\d{3}\-\d{2}",          // CPF
                @"[A-Z]{2}\d{4}",                       // Código simples
                @"^\d+$",                               // Apenas números
                @"^[a-zA-Z0-9]+$"                       // Alfanumérico
            ));
        }
    }
}
