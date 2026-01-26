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
        private readonly IBaseRepository<RegFileName> _fileNameRepository;
        private readonly IMapper _mapper;

        public CreateRegIrpfHandler(IBaseRepository<RegFileName> filename, IMapper mapper)
        {
            _fileNameRepository = filename;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegIrpfCommand request, CancellationToken cancellationToken)
        {

            var irpfList = _mapper.Map<RegFileName>(request.Requests) ?? throw new InvalidDataException("Invalid data");

            await _fileNameRepository.AddAsync(irpfList);
            await _fileNameRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Irpfs Criadas com Sucesso"
            };
        }
    }
}