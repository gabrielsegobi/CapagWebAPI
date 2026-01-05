using Application.Commands.ICPLimits;
using Application.Exceptions.ICPLimits;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ICPLimits
{
    public class UpdateICPLimitHandler : IRequestHandler<UpdateICPLimitCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<ICPLimit> _baseRepository;
        private readonly IMapper _mapper;

        public UpdateICPLimitHandler(IBaseRepository<ICPLimit> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        public async Task<UpdateApiResponse> Handle(UpdateICPLimitCommand request, CancellationToken cancellationToken)
        {
            var icp = await _baseRepository.GetByIdAsync(request.Id);
            if (icp == null)
            {
                throw new ICPLimitNotFoundException(request.Id);
            }
         
            var icpToUpdate = _mapper.Map(request.UpdateICPLimitRequest, icp);

            _baseRepository.Update(icpToUpdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = " ICP atualizado com sucesso." };
        }
    }
}
