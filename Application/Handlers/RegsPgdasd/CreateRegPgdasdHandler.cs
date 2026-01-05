using Application.Commands.RegsPgdasd;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegsPgdasd
{
    public class CreateRegPgdasdHandler : IRequestHandler<CreateRegPgdasdCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegPgdasd> _baseRepository;
        private readonly IBaseRepository<RegFileName> _fileNameRepository;
        private readonly IMapper _mapper;

        public CreateRegPgdasdHandler(IBaseRepository<RegPgdasd> baseRepository, IBaseRepository<RegFileName> fileNameRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _fileNameRepository = fileNameRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegPgdasdCommand request, CancellationToken cancellationToken)
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

            var pgdasdList = _mapper.Map<List<RegPgdasd>>(request.Requests) ?? throw new InvalidDataException("Invalid data");

            foreach (var item in pgdasdList)
            {
                item.IdFilename = regFileName.Id;
            }

            await _baseRepository.AddRangeAsync(pgdasdList);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Pgdasds Criados com Sucesso"
            };
        }
    }
}
