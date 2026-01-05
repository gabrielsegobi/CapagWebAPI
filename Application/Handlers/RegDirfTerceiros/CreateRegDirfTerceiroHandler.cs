using Application.Commands.RegDirfTerceiros;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDirfTerceiros
{
    public class CreateRegDirfTerceiroHandler : IRequestHandler<CreateRegDirfTerceiroCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegDirfTerceiro> _baseRepository;
        private readonly IBaseRepository<RegFileName> _fileNameRepository;

        private readonly IMapper _mapper;

        public CreateRegDirfTerceiroHandler(IBaseRepository<RegDirfTerceiro> baseRepository, IBaseRepository<RegFileName> filename, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _fileNameRepository = filename;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegDirfTerceiroCommand request, CancellationToken cancellationToken)
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

            var DirfList = _mapper.Map<List<RegDirfTerceiro>>(request.Requests) ?? throw new InvalidDataException("Invalid data"); ;

            foreach (var item in DirfList)
            {
                item.IdFilename = regFileName.Id;
            }
            await _baseRepository.AddRangeAsync(DirfList);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Dirfs Criadas com Sucesso"
            };
        }
    }
}
