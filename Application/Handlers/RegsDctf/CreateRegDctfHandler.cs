using Application.Commands.RegsDctf;
using Application.Exceptions.RegsDctf;
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
        private readonly IBaseRepository<RegDctf> _regDctfRepository;
        private readonly IMapper _mapper;

        public CreateRegDctfHandler(IBaseRepository<RegFileName> filename, IMapper mapper, IBaseRepository<RegDctf> regDctfRepository)
        {
            _fileNameRepository = filename;
            _mapper = mapper;
            _regDctfRepository = regDctfRepository;
        }

        public async Task<CreateApiResponse> Handle(CreateRegDctfCommand request, CancellationToken cancellationToken)
        {
            var periodo = request.Requests.Requests[0].Periodo;
            var idEmpresa = request.Requests.Requests[0].IdEmpresa;
            var isRetificadora = !string.IsNullOrWhiteSpace(request.Requests.Requests[0].ReciboRetificadora);


            if (isRetificadora)
            {
                var regFileNames = _fileNameRepository
                        .Query(x => x.RegDctfs.Any(d => d.IdEmpresa == idEmpresa && d.Periodo == periodo))
                        .ToList();

                _fileNameRepository.DeleteRange(regFileNames);
            }
            else
            {
                if (await _regDctfRepository.AnyAsync(rd => rd.IdEmpresa == idEmpresa && rd.Periodo == periodo))
                    throw new DuplicateDctfPeriodException(periodo);
            }

            var dctfList = _mapper.Map<RegFileName>(request.Requests) ?? throw new InvalidDataException("Invalid data");

            await _fileNameRepository.AddAsync(dctfList);
            await _fileNameRepository.SaveChangesAsync();

            return new CreateApiResponse("Dctfs Criados com Sucesso", dctfList.Id);
        }
    }
}
