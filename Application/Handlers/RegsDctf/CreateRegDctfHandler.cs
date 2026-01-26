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
        private readonly IBaseRepository<RegFileName> _fileNameRepository;

        private readonly IMapper _mapper;

        public CreateRegDctfHandler(IBaseRepository<RegFileName> filename, IMapper mapper)
        {
            _fileNameRepository = filename;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegDctfCommand request, CancellationToken cancellationToken)
        {
            var dctfList = _mapper.Map<RegFileName>(request.Requests) ?? throw new InvalidDataException("Invalid data");

            await _fileNameRepository.AddAsync(dctfList);
            await _fileNameRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Dctfs Criados com Sucesso"
            };
        }
    }
}
