using Application.Commands.DemonstrativosContabeis;
using Application.Commands.Indicadores;
using Application.Commands.ResultadosIndicesICP;
using Application.Exceptions.Empresas;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Entities;
using Infrastructure.Helpers;
using Infrastructure.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Handlers.DemonstrativosContabeis
{
    public class CadastrarDemonstrativosContabeisHandler
        : IRequestHandler<CadastrarDemonstrativosContabeisCommand, CadastrarDemonstrativosContabeisResponse>
    {
        private readonly IBaseRepository<Empresa> _empresaRepository;
        private readonly IBaseRepository<DemonstrativoContabil> _demonstrativoRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly Infrastructure.Context.CPGDbContext _db;
        private readonly IMediator _mediator;

        public CadastrarDemonstrativosContabeisHandler(
            IBaseRepository<Empresa> empresaRepository,
            IBaseRepository<DemonstrativoContabil> demonstrativoRepository,
            ICurrentUserService currentUser,
            Infrastructure.Context.CPGDbContext db,
            IMediator mediator)
        {
            _empresaRepository = empresaRepository;
            _demonstrativoRepository = demonstrativoRepository;
            _currentUser = currentUser;
            _db = db;
            _mediator = mediator;
        }

        public async Task<CadastrarDemonstrativosContabeisResponse> Handle(
            CadastrarDemonstrativosContabeisCommand command,
            CancellationToken cancellationToken)
        {
            if (command.Contas == null || command.Contas.Count == 0)
                throw new ArgumentException("Lista de contas inválida.");

            var tenantHeader = _currentUser.TenantId;
            if (!tenantHeader.HasValue)
                throw new ArgumentException("Tenant não informado.");

            if (command.IdTenant != tenantHeader.Value)
                throw new ArgumentException("id_tenant não confere com o tenant do header.");

            var idTenant = tenantHeader.Value;
            var idEmpresa = command.Contas[0].IdEmpresa;

            if (idEmpresa <= 0)
                throw new ArgumentException("id_empresa inválido.");

            if (command.Contas.Any(c => c.IdEmpresa != idEmpresa))
                throw new ArgumentException("Todas as contas devem pertencer à mesma empresa.");

            ValidarContas(command.Contas);

            var empresa = await _empresaRepository.GetByIdAsync(idEmpresa);
            if (empresa == null || empresa.IdTenant != idTenant)
                throw new EmpresaNotFoundException(idEmpresa);

            var response = new CadastrarDemonstrativosContabeisResponse
            {
                IdEmpresa = idEmpresa,
                Sucesso = false
            };

            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var now = DateTimeHelper.GetDateTimeNow();

                if (command.Sobrescrever)
                {
                    var chavesSobrescrita = command.Contas
                        .Select(c => (c.Ano, PerApur: c.PerApur.Trim(), TipoTrib: c.TipoTrib.Trim()))
                        .Distinct()
                        .ToList();

                    var existentes = await _demonstrativoRepository
                        .Query(dc => dc.IdTenant == idTenant &&
                                     dc.IdEmpresa == idEmpresa &&
                                     dc.DeletedAt == null,
                            asNoTracking: false)
                        .ToListAsync(cancellationToken);

                    var paraDeletar = existentes
                        .Where(dc => chavesSobrescrita.Any(k =>
                            dc.Ano == k.Ano &&
                            dc.PerApur == k.PerApur &&
                            dc.TipoTrib == k.TipoTrib))
                        .ToList();

                    foreach (var item in paraDeletar)
                    {
                        item.DeletedAt = now;
                        item.UpdatedAt = now;
                    }

                    if (paraDeletar.Count > 0)
                    {
                        _demonstrativoRepository.UpdateRange(paraDeletar);
                        await _demonstrativoRepository.SaveChangesAsync();
                    }

                    response.TotalDeletados = paraDeletar.Count;
                }

                await EnriquecerValCtaRefIniAsync(command.Contas, idTenant, idEmpresa, cancellationToken);

                // Descarta registros sem dados úteis: ambos val_cta_ref_ini e val_cta_ref_fin zerados/nulos.
                var contasValidas = command.Contas
                    .Where(c => !AmbosZerados(c.ValCtaRefIni, c.ValCtaRefFin))
                    .ToList();

                var entidades = contasValidas
                    .Select(item => MapToEntity(item, idTenant, now))
                    .ToList();

                await _demonstrativoRepository.AddRangeAsync(entidades);
                await _demonstrativoRepository.SaveChangesAsync();

                await tx.CommitAsync(cancellationToken);

                response.TotalInseridos = entidades.Count;
                response.Sucesso = true;
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(cancellationToken);
                response.Erro = ex.Message;
                return response;
            }

            await _mediator.Send(new CalcIndicadoresCommand { IdEmpresa = idEmpresa }, cancellationToken);
            await _mediator.Send(new CalcResultadosIndicesICPCommand { IdEmpresa = idEmpresa }, cancellationToken);

            empresa.DadosProcessados = true;
            empresa.UpdatedAt = DateTimeHelper.GetDateTimeNow();
            _empresaRepository.Update(empresa);
            await _empresaRepository.SaveChangesAsync();

            return response;
        }

        /// <summary>
        /// Para itens de balanço (código não começa com "3") que chegaram sem val_cta_ref_ini,
        /// tenta preencher com o fechamento (val_cta_ref_fin / ind) do ano N-1.
        /// Prioridade: payload primeiro, banco em seguida.
        /// </summary>
        private async Task EnriquecerValCtaRefIniAsync(
            List<CadastrarDemonstrativosContabeisItem> contas,
            long idTenant,
            long idEmpresa,
            CancellationToken cancellationToken)
        {
            var elegíveis = contas
                .Where(c => c.ValCtaRefIni == null && !c.Codigo.TrimStart().StartsWith("3"))
                .ToList();

            if (elegíveis.Count == 0)
                return;

            // Fechamento disponível no próprio payload: (Ano, Codigo) -> item
            // Inclui val_cta_ref_fin = 0 (0 é saldo válido de fechamento).
            var fechamentoPayload = contas
                .Where(c => c.ValCtaRefFin.HasValue && !c.Codigo.TrimStart().StartsWith("3"))
                .GroupBy(c => (c.Ano, Codigo: c.Codigo.Trim()))
                .ToDictionary(g => g.Key, g => SelecionarFechamento(g.ToList()));

            // Anos N-1 que ainda precisamos buscar no banco
            var anosNecessariosDb = elegíveis
                .Select(c => c.Ano - 1)
                .Distinct()
                .Where(ano => !fechamentoPayload.Keys.Any(k => k.Ano == ano))
                .ToList();

            // Fechamento do banco por (Ano, Codigo)
            var fechamentoDb = new Dictionary<(int Ano, string Codigo), CadastrarDemonstrativosContabeisItem>();

            if (anosNecessariosDb.Count > 0)
            {
                var registrosBanco = await _demonstrativoRepository
                    .Query(dc => dc.IdTenant == idTenant &&
                                 dc.IdEmpresa == idEmpresa &&
                                 dc.DeletedAt == null &&
                                 anosNecessariosDb.Contains(dc.Ano) &&
                                 dc.ValCtaRefFin.HasValue &&
                                 !dc.Codigo.StartsWith("3"))
                    .Select(dc => new CadastrarDemonstrativosContabeisItem
                    {
                        Ano = dc.Ano,
                        Codigo = dc.Codigo,
                        PerApur = dc.PerApur ?? string.Empty,
                        ValCtaRefFin = dc.ValCtaRefFin,
                        IndValCtaRefFin = dc.IndValCtaRefFin
                    })
                    .ToListAsync(cancellationToken);

                fechamentoDb = registrosBanco
                    .GroupBy(dc => (dc.Ano, Codigo: dc.Codigo.Trim()))
                    .ToDictionary(g => g.Key, g => SelecionarFechamento(g.ToList()));
            }

            foreach (var item in elegíveis)
            {
                var chaveAnterior = (Ano: item.Ano - 1, Codigo: item.Codigo.Trim());

                CadastrarDemonstrativosContabeisItem? fonte = null;
                if (fechamentoPayload.TryGetValue(chaveAnterior, out var fp))
                    fonte = fp;
                else if (fechamentoDb.TryGetValue(chaveAnterior, out var fd))
                    fonte = fd;

                if (fonte == null)
                {
                    // Sem dado de N-1: abertura zerada.
                    item.ValCtaRefIni = 0m;
                    continue;
                }

                item.ValCtaRefIni = fonte.ValCtaRefFin ?? 0m;
                item.IndValCtaRefIni = fonte.IndValCtaRefFin;
            }
        }

        /// <summary>
        /// Retorna true quando ambos os saldos são nulos ou zero — registro sem dado útil.
        /// </summary>
        private static bool AmbosZerados(decimal? ini, decimal? fin) =>
            (ini == null || ini == 0m) && (fin == null || fin == 0m);

        /// <summary>
        /// Seleciona o registro representativo do fechamento de um exercício entre vários períodos:
        /// A00 > T04 > maior per_apur disponível.
        /// </summary>
        private static CadastrarDemonstrativosContabeisItem SelecionarFechamento(
            List<CadastrarDemonstrativosContabeisItem> itens)
        {
            return itens.FirstOrDefault(x => x.PerApur.Trim() == "A00")
                ?? itens.FirstOrDefault(x => x.PerApur.Trim() == "T04")
                ?? itens.OrderByDescending(x => x.PerApur.Trim()).First();
        }

        private static void ValidarContas(IReadOnlyList<CadastrarDemonstrativosContabeisItem> contas)
        {
            for (var i = 0; i < contas.Count; i++)
            {
                var item = contas[i];
                var prefixo = $"Conta[{i}]";

                if (string.IsNullOrWhiteSpace(item.Codigo))
                    throw new ArgumentException($"{prefixo}: codigo é obrigatório.");

                if (string.IsNullOrWhiteSpace(item.PerApur))
                    throw new ArgumentException($"{prefixo}: per_apur é obrigatório.");

                if (item.Ano <= 0)
                    throw new ArgumentException($"{prefixo}: ano inválido.");

                if (string.IsNullOrWhiteSpace(item.TipoTrib))
                    throw new ArgumentException($"{prefixo}: tipo_trib é obrigatório.");

                if (item.Tipo == default)
                    throw new ArgumentException($"{prefixo}: tipo é obrigatório.");
            }
        }

        private static DemonstrativoContabil MapToEntity(
            CadastrarDemonstrativosContabeisItem item,
            long idTenant,
            DateTime now)
        {
            return new DemonstrativoContabil
            {
                IdTenant = idTenant,
                IdEmpresa = item.IdEmpresa,
                DtIni = item.DtIni,
                DtIniApur = item.DtIniApur,
                DtFinApur = item.DtFinApur,
                PerApur = item.PerApur.Trim(),
                Ano = item.Ano,
                Codigo = item.Codigo.Trim(),
                Descricao = item.Descricao?.Trim() ?? string.Empty,
                Tipo = item.Tipo,
                Nivel = item.Nivel,
                ValCtaRefIni = item.ValCtaRefIni,
                IndValCtaRefIni = item.IndValCtaRefIni,
                ValCtaRefDeb = item.ValCtaRefDeb,
                ValCtaRefCred = item.ValCtaRefCred,
                ValCtaRefFin = item.ValCtaRefFin,
                IndValCtaRefFin = item.IndValCtaRefFin,
                TipoTrib = item.TipoTrib.Trim(),
                CreatedAt = now,
                UpdatedAt = now,
                DeletedAt = null
            };
        }
    }
}
