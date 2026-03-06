using Bogus;
using Domain.Contracts.ValidatioRegexes;

namespace TestSupport.Fakes.ValidationRegexes
{
    public class UpdateValidationRegexRequestFaker : Faker<UpdateValidationRegexRequest>
    {
        public UpdateValidationRegexRequestFaker()
        {
            RuleFor(x => x.Id, f => f.Random.Long(1, long.MaxValue));

            RuleFor(x => x.Regex, f => f.PickRandom(
                @"\d{2}\/\d{2}\/\d{4}",
                @"\d{3}\.\d{3}\.\d{3}\-\d{2}",
                @"[A-Z]{2}\d{4}",
                @"^\d+$",
                @"^[a-zA-Z0-9]+$"
            ));
        }
    }
}
