using Application.Commands.RegsIrpf;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegsIrpf
{
    public class CreateRegIrpfHandler : IRequestHandler<CreateRegIrpfCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegIrpf> _baseRepository;
        private readonly IBaseRepository<RegFileName> _fileNameRepository;

        private readonly IMapper _mapper;

        public CreateRegIrpfHandler(IBaseRepository<RegIrpf> baseRepository, IBaseRepository<RegFileName> filename, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _fileNameRepository = filename;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegIrpfCommand request, CancellationToken cancellationToken)
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

            var irpfList = _mapper.Map<List<RegIrpf>>(request.Requests) ?? throw new InvalidDataException("Invalid data");

            foreach (var item in irpfList)
            {
                item.IdFilename = regFileName.Id;
            }

            await _baseRepository.AddRangeAsync(irpfList);
            await _baseRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Irpfs Criadas com Sucesso"
            };
        }
    }
}
