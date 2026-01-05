using Application.Exceptions;
using Application.Exceptions.ModelosIndicesICP;
using Application.Queries.ModelosIndicesICP;
using AutoMapper;
using Domain.Contracts.ModelosIndicesICP;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.ModelosIndicesICP
{
    public class GetModeloIndiceICPByIdHandler : IRequestHandler<GetModeloIndiceICPByIdQuery, GetApiResponse>
    {
        private readonly IBaseRepository<ModeloIndiceICP> _baseRepository;
        private readonly IMapper _mapper;
        public GetModeloIndiceICPByIdHandler(IBaseRepository<ModeloIndiceICP> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<GetApiResponse> Handle(GetModeloIndiceICPByIdQuery request, CancellationToken cancellationToken)
        {
            var modelo = await _baseRepository.Query().Include(i => i.Resultados).FirstOrDefaultAsync(x => x.IdModeloIndice == request.Id, cancellationToken);

            if (modelo == null)
            {
                throw new ModeloIndiceICPNotFoundException(request.Id);
            }

            var result = _mapper.Map<ModeloIndiceICPDto>(modelo);
            return new GetApiResponse
            {
                Data = result,
                Message = "Modelo encontrado com sucesso"
            };
        }
    }
}
