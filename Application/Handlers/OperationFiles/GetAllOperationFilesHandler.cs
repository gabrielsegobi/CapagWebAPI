using Application.Queries.OperationsFiles;
using AutoMapper;
using Domain.Contracts.OperationFiles;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.OperationFiles
{
    public class GetAllOperationFilesHandler : IRequestHandler<GetAllOperationFilesQuery, PagedApiResponse<OperationFilesDto>>
    {
        private readonly IBaseRepository<OperationFile> _operationFileRepository;
        private readonly IMapper _mapper;

        public GetAllOperationFilesHandler(IBaseRepository<OperationFile> operationFileRepository, IMapper mapper)
        {
            _operationFileRepository = operationFileRepository;
            _mapper = mapper;
        }

        public async Task<PagedApiResponse<OperationFilesDto>> Handle(GetAllOperationFilesQuery request, CancellationToken cancellationToken)
        {
            var query = _operationFileRepository.Query();

            var pagedResult = await query.ReadPage<OperationFile, OperationFilesDto>(
                request.Filter,
                applyFilters: q =>
                {
                    if (request.Filter.IdEmpresa >= 0)
                        q = q.Where(e => e.Operation.IdEmpresa == request.Filter.IdEmpresa);



                    return q;
                },
                mapFunc: data => _mapper.Map<IEnumerable<OperationFilesDto>>(data)
            );

            return pagedResult;
        }
    }
}
