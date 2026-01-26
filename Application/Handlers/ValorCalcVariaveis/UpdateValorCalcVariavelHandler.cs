using Application.Commands.ValorCalcVariaveis;
using Application.Exceptions.ValorCalcVariaveis;
using AutoMapper;
using Domain.Contracts.Responses;
using Domain.Entities;
using Infrastructure.Interface;
using MediatR;

namespace Application.Handlers.ValorCalcVariaveis
{

    public class UpdateValorCalcVariavelHandler : IRequestHandler<UpdateValorCalcVariavelCommand, UpdateApiResponse>
    {
        private readonly IBaseRepository<ValorCalcVariavel> _baseRepository;
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IMapper _mapper;

        public UpdateValorCalcVariavelHandler(IBaseRepository<ValorCalcVariavel> baseRepository, IBaseRepository<Empresa> empresaRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _empresaRepository = empresaRepository;
            _mapper = mapper;
        }

        public async Task<UpdateApiResponse> Handle(UpdateValorCalcVariavelCommand request, CancellationToken cancellationToken)
        {
            var calc = await _baseRepository.GetByIdAsync(request.Id)
                ?? throw new ValorCalcVariavelNotFoundException(request.Id);

            var calcToupdate = _mapper.Map(request.Request, calc);

            _baseRepository.Update(calcToupdate);
            await _baseRepository.SaveChangesAsync();

            return new UpdateApiResponse { Message = "Valor Atualizado com sucesso" };
        }
    }
}
