using Antlr.Runtime.Tree;
using Application.Commands.ICPAnterior;
using Application.Exceptions.ICPAnterior;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ICPAnterior
{
    public class UpdateICPAnteriorHandler : IRequestHandler<UpdateICPAnteriorCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<ICPsAnterior> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateICPAnteriorHandler(IBaseRepository<ICPsAnterior> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateICPAnteriorCommand request, CancellationToken cancellationToken)
        {
            var ICP = await _baseRepository.GetFirstOrDefaultAsync(i => i.IdIcpAnterior == request.Id) ?? throw new ICPAnteriorNotFoundException(request.Id);
            var ICPToUpdate = _mapper.Map(request.Request, ICP);

            _baseRepository.Update(ICPToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "ICP Anterior atualizado com sucesso." };
        }
    }
}
