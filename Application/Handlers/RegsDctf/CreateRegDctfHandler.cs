using Application.Commands.RegsDctf;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegsDctf
{
    public class CreateRegDctfHandler : IRequestHandler<CreateRegDctfCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegDctf> _baseRepository;
        private readonly IBaseRepository<RegFileName> _fileNameRepository;

        private readonly IMapper _mapper;

        public CreateRegDctfHandler(IBaseRepository<RegDctf> baseRepository, IBaseRepository<RegFileName> filename, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _fileNameRepository = filename;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegDctfCommand request, CancellationToken cancellationToken)
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

            var dctfList = _mapper.Map<List<RegDctf>>(request.Requests) ?? throw new InvalidDataException("Invalid data");

            foreach (var item in dctfList)
            {
                item.IdFilename = regFileName.Id;
            }

            await _baseRepository.AddRangeAsync(dctfList);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Dctfs Criados com Sucesso"
            };
        }
    }
}
