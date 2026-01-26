using Application.Commands.RegDefis;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.RegDefis
{
    public class CreateRegDefisHandler : IRequestHandler<CreateRegDefisCommand, CreateApiResponse>
    {
        private readonly IBaseRepository<RegDefi> _baseRepository;
        private readonly IBaseRepository<RegFileName> _fileNameRepository;
        private readonly IMapper _mapper;

        public CreateRegDefisHandler(IBaseRepository<RegDefi> baseRepository, IBaseRepository<RegFileName> fileNameRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _fileNameRepository = fileNameRepository;
            _mapper = mapper;
        }

        public async Task<CreateApiResponse> Handle(CreateRegDefisCommand request, CancellationToken cancellationToken)
        {
            //var regFileName = new RegFileName
            //{
               
            //    FileName = request.FileName,
            //    Type = request.Type
            //};

            //await _fileNameRepository.AddAsync(regFileName);
            //await _fileNameRepository.SaveChangesAsync();

            //if (request.Requests == null || request.Requests.Count == 0)
            //    throw new InvalidDataException("Nenhum registro enviado");

            //var DefisList = _mapper.Map<List<RegDefi>>(request.Request) ?? throw new InvalidDataException("Invalid data");
            var DefisList = _mapper.Map<RegFileName>(request.Request) ?? throw new InvalidDataException("Invalid data");
     
            //foreach (var item in DefisList)
            //{
            //    item.IdFilename = regFileName.Id;
            //}

            //await _baseRepository.AddRangeAsync(DefisList);
            //await _baseRepository.SaveChangesAsync();

            await _fileNameRepository.AddAsync(DefisList);
            await _fileNameRepository.SaveChangesAsync();

            return new CreateApiResponse
            {
                Message = "Defis Criadas com Sucesso"
            };
        }
    }
}
