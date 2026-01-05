using Application.Commands.RegDarf;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDarf
{
    public class CreateRegDarfHandler : IRequestHandler<CreateRegDarfCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegDarfs> _baseRepository;
        private readonly IBaseRepository<RegFileName> _fileNameRepository;

        private readonly IMapper _mapper;

        public CreateRegDarfHandler(IBaseRepository<RegDarfs> baseRepository, IBaseRepository<RegFileName> filename, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _fileNameRepository = filename;
            _mapper = mapper;
        }
        public async Task<CreateApiResponse> Handle(CreateRegDarfCommand request, CancellationToken cancellationToken)
        {
            var regFileName = new RegFileName
            {
                Id = Random.Shared.NextInt64(1, long.MaxValue),
                FileName = request.FileName,
                Type = request.Type
            };

            await _fileNameRepository.AddAsync(regFileName);
            await _fileNameRepository.SaveChangesAsync();

            if (request.Requests == null || request.Requests.Count == 0)
                throw new InvalidDataException("Nenhum registro enviado");

            var darfList = _mapper.Map<List<RegDarfs>>(request.Requests) ?? throw new InvalidDataException("Invalid data"); ;

            foreach (var item in darfList)
            {
                item.IdFilename = regFileName.Id;
            }

            await _baseRepository.AddRangeAsync(darfList);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Darfs Criadas com Sucesso"
            };
        }
    }
}
