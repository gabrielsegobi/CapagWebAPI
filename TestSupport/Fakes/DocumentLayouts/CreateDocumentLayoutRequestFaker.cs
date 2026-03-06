using Bogus;
using Domain.Contracts.DocumentsLayouts;
using TestSupport.Fakes.ValidationRegexes;

namespace TestSupport.Fakes.DocumentLayouts
{
    public class CreateDocumentLayoutRequestFaker : Faker<CreateDocumentLayoutRequest>
    {
        private readonly CreateValidationRegexRequestFaker _validationRegexFaker;
        public CreateDocumentLayoutRequestFaker()
        {
            _validationRegexFaker = new CreateValidationRegexRequestFaker();

            RuleFor(x => x.LayoutName, f => f.Company.CompanyName());

            RuleFor(x => x.Description, f =>
                f.Random.Bool() ? f.Lorem.Sentence() : null);

            RuleFor(x => x.ValidationRegex, f =>
                _validationRegexFaker.Generate().Regex);

            RuleFor(x => x.System, f => f.Random.Bool());

            RuleFor(x => x.ValidationRegexes, f =>
                f.Random.Bool()
                    ? _validationRegexFaker.Generate(f.Random.Int(1, 3))
                    : null
            );

        }

        public CreateDocumentLayoutRequestFaker WithValidationRegex()
        {
            RuleFor(x => x.ValidationRegexes, f =>
                _validationRegexFaker.Generate(f.Random.Int(1, 3))
            );

            return this;
        }

        public CreateDocumentLayoutRequestFaker WithoutValidationRegex()
        {
            RuleFor(x => x.ValidationRegexes, f => null);
            return this;
        }
    }
}
