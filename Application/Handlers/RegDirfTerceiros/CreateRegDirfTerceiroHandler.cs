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
        private readonly IBaseRepository<RegFileName> _fileNameRepository;

        private readonly IMapper _mapper;

        public CreateRegDirfTerceiroHandler(IBaseRepository<RegFileName> filename, IMapper mapper)
        {
            _fileNameRepository = filename;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegDirfTerceiroCommand request, CancellationToken cancellationToken)
        {
            var DirfList = _mapper.Map<RegFileName>(request.Requests) ?? throw new InvalidDataException("Invalid data");

            await _fileNameRepository.AddAsync(DirfList);
            await _fileNameRepository.SaveChangesAsync();

            return new CreateApiResponse("Dirfs Criadas com Sucesso", DirfList.Id);
        }
    }
}
