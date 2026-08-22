using Application.Commands.DemonstrativosContabeis;
using Application.Commands.Indicadores;
using Application.Commands.ResultadosIndicesICP;
using Application.Exceptions.Empresas;
using Application.Helpers;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Context;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class CreateDemonstrativoContabilHandler : IRequestHandler<CreateDemonstrativoContabilCommand>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<DemonstrativoContabil> _demonstrativoRepository;
        private readonly IBaseRepository<RegimeTributario> _tributarioRepository;
        private readonly CPGDbContext _db;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IIntegracaoDemonstrativosService _integracaoService;

        public CreateDemonstrativoContabilHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<DemonstrativoContabil> demonstrativoRepository,
            IBaseRepository<RegimeTributario> tributarioRepository,
            CPGDbContext db,
            IMediator mediator,
            IIntegracaoDemonstrativosService integracaoService,
            IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _demonstrativoRepository = demonstrativoRepository;
            _tributarioRepository = tributarioRepository;
            _db = db;
            _mediator = mediator;
            _mapper = mapper;
            _integracaoService = integracaoService;
        }

        public async Task Handle(CreateDemonstrativoContabilCommand request, CancellationToken cancellationToken)
        {
            var empresa = await _empresaRepository.GetByIdAsync(request.IdEmpresa);

            if (empresa == null)
                throw new EmpresaNotFoundException(request.IdEmpresa);

            await using var processLock = await EmpresaProcessamentoLock.AdquirirAsync(
                _db.Database,
                empresa.IdEmpresa,
                cancellationToken);

            try
            {
                var dadosTributacao = await _integracaoService.ObterTributacoesAsync(empresa, cancellationToken);
                if (dadosTributacao == null || !dadosTributacao.Any())
                    throw new Exception("Nenhum regime tributário retornado pela API GMaster.");

                var anosImportacao = DemonstrativosAnosHelper.ObterAnosImportacaoComAnterior(
                    dadosTributacao.Select(t => t.Ano));

                if (anosImportacao.Count == 0)
                    throw new Exception("Não foi possível determinar anos de importação a partir da tributação.");

                var taskDre = _integracaoService.ObterDreAsync(empresa, anosImportacao, cancellationToken);
                var taskBalanco = _integracaoService.ObterBalancoAsync(empresa, anosImportacao, cancellationToken);

                await Task.WhenAll(taskDre, taskBalanco);

                var dadosDRE = await taskDre;
                var dadosBalanco = await taskBalanco;

                if ((dadosDRE == null || !dadosDRE.Any()) ||
                    (dadosBalanco == null || !dadosBalanco.Any()))
                    throw new Exception("Falha ao processar dados de DRE ou Balanço.");

                var demonstrativos = new List<DemonstrativoContabil>();
                demonstrativos.AddRange(_mapper.Map<List<DemonstrativoContabil>>(dadosDRE));
                demonstrativos.AddRange(_mapper.Map<List<DemonstrativoContabil>>(dadosBalanco));
                NormalizarDemonstrativos(demonstrativos);

                if (demonstrativos.Count == 0)
                    throw new Exception("Mapeamento DRE/Balanço não produziu demonstrativos.");

                var previousTimeout = _db.Database.GetCommandTimeout();
                _db.Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

                try
                {
                    await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
                    try
                    {
                        // ExecuteDelete bypassa change tracker e AuditInterceptor (idempotente e leve).
                        await _db.Set<DemonstrativoContabil>()
                            .Where(d => d.IdTenant == empresa.IdTenant && d.IdEmpresa == empresa.IdEmpresa)
                            .ExecuteDeleteAsync(cancellationToken);

                        await _db.Set<RegimeTributario>()
                            .Where(r => r.IdTenant == empresa.IdTenant && r.IdEmpresa == empresa.IdEmpresa)
                            .ExecuteDeleteAsync(cancellationToken);

                        // Lotes menores: menos parâmetros por comando e menor risco de packet/transação.
                        await InserirEmLotesAsync(demonstrativos, batchSize: 50, cancellationToken);

                        await _tributarioRepository.AddRangeAsync(_mapper.Map<List<RegimeTributario>>(dadosTributacao));
                        await _tributarioRepository.SaveChangesAsync();

                        await transaction.CommitAsync(cancellationToken);
                    }
                    catch
                    {
                        try
                        {
                            await transaction.RollbackAsync(cancellationToken);
                        }
                        catch
                        {
                            // Transação já abortada pelo MySQL — ignorar segundo erro no rollback.
                        }

                        throw;
                    }
                }
                finally
                {
                    _db.Database.SetCommandTimeout(previousTimeout);
                }

                var gravados = await _db.Set<DemonstrativoContabil>()
                    .AsNoTracking()
                    .CountAsync(d => d.IdTenant == empresa.IdTenant && d.IdEmpresa == empresa.IdEmpresa, cancellationToken);

                if (gravados == 0)
                    throw new Exception("Commit concluído, mas nenhum demonstrativo persistiu no banco.");

                await _mediator.Send(new CalcIndicadoresCommand
                {
                    IdEmpresa = request.IdEmpresa
                }, cancellationToken);

                await _mediator.Send(new CalcResultadosIndicesICPCommand
                {
                    IdEmpresa = request.IdEmpresa
                }, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Falha ao gerar demonstrativos contábeis. Erro: {MensagemCompleta(ex)}", ex);
            }
        }

        private async Task InserirEmLotesAsync(
            List<DemonstrativoContabil> demonstrativos,
            int batchSize,
            CancellationToken cancellationToken)
        {
            for (var i = 0; i < demonstrativos.Count; i += batchSize)
            {
                var lote = demonstrativos.Skip(i).Take(batchSize).ToList();
                await _demonstrativoRepository.AddRangeAsync(lote);
                await _demonstrativoRepository.SaveChangesAsync();
                _db.ChangeTracker.Clear();
            }
        }

        private static void NormalizarDemonstrativos(IEnumerable<DemonstrativoContabil> demonstrativos)
        {
            foreach (var d in demonstrativos)
            {
                d.Descricao = SanitizarTexto(d.Descricao, 255);
                d.Codigo = SanitizarTexto(d.Codigo, 20);
                d.PerApur = SanitizarTexto(d.PerApur, 10);
                d.TipoTrib = SanitizarTexto(d.TipoTrib, 50);

                // CHAR(1) com '\0' quebra insert MySQL em alguns collations.
                if (d.Tipo == '\0')
                    d.Tipo = ' ';

                if (d.IndValCtaRefIni == '\0')
                    d.IndValCtaRefIni = null;

                if (d.IndValCtaRefFin == '\0')
                    d.IndValCtaRefFin = null;
            }
        }

        /// <summary>
        /// Remove NUL e controles que corrompem o protocolo MySQL/Pomelo e truncam o campo.
        /// </summary>
        private static string SanitizarTexto(string? valor, int maxLen)
        {
            if (string.IsNullOrEmpty(valor))
                return string.Empty;

            var sb = new StringBuilder(Math.Min(valor.Length, maxLen));
            foreach (var ch in valor)
            {
                if (ch == '\0' || (char.IsControl(ch) && ch != '\t' && ch != '\n' && ch != '\r'))
                    continue;

                sb.Append(ch);
                if (sb.Length >= maxLen)
                    break;
            }

            return sb.ToString().Trim();
        }

        private static string MensagemCompleta(Exception ex)
        {
            var partes = new List<string>();
            for (var atual = ex; atual != null; atual = atual.InnerException)
            {
                if (!string.IsNullOrWhiteSpace(atual.Message))
                    partes.Add(atual.Message);
            }

            return string.Join(" | ", partes.Distinct());
        }
    }
}
