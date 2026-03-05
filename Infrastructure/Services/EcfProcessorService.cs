using Domain.Contracts;
using Domain.Entities;
using Domain.Entities.Sped;
using Domain.Entities.Sped.Ecf;
using Domain.Interfaces;
using Infrastructure.Interface;
using Infrastructure.Parsers.Sped;
using Infrastructure.Writers.Sped;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Ocsp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using MySqlConnector;
namespace Infrastructure.Services
{
    public class EcfProcessorService : IEcfProcessorService
    {
        //private readonly IGetRelationShip _relationship;
        //private readonly IObjectStorage _objectStorage;
        //private readonly IEcfFactory _ecfFactory;
        //private readonly IBulkInsertService _bulkInsertService;

        //private readonly IBaseRepository<OperationFile> _operationFileRepository;
        //private readonly IBaseRepository<Operation> _operationRepository;
        //private readonly IBaseRepository<E_0000> e_0000Repository;


        //private readonly Dictionary<Type, IList<EcfBase>> _buffer = new();

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILayoutRepository _layoutRepository;
        //private readonly ChannelWriter<ParsedRow> _writer;
        public EcfProcessorService(IServiceScopeFactory scopeFactory, ILayoutRepository layoutRepository)
        {
            _scopeFactory = scopeFactory;
            _layoutRepository = layoutRepository;
            //_writer = writer;
        }







        //var relationships = await _relationship.ExecuteAsync("ecf", default);
        //var hierarchy = Load(relationships);


        public async Task ProcessAsync(long IdOp, long IdEmpresa, long IdTenant, long FileId, string FileName, CancellationToken cancellationToken, bool overwrite = false)
        {

            using var scope = _scopeFactory.CreateScope();

            var _operationFileRepository = scope.ServiceProvider.GetRequiredService<IBaseRepository<OperationFile>>();
            var e_0000Repository = scope.ServiceProvider.GetRequiredService<IBaseRepository<E_0000>>();
            var _objectStorage = scope.ServiceProvider.GetRequiredService<IObjectStorage>();
            var _ecfFactory = scope.ServiceProvider.GetRequiredService<IEcfFactory>();
            var _bulkInsertService = scope.ServiceProvider.GetRequiredService<IBulkInsertService>();

            var _buffer = new Dictionary<Type, IList<EcfBase>>();

            var hierarchy = Create();

            var stack = new Stack<StackItem>(capacity: 32);

            var totalWatch = Stopwatch.StartNew();
            var ecfs = new List<EcfBase>();


            var _connectionString = "server=192.168.0.184; port=3306; database=gsaas; user=user; password=password; AllowLoadLocalInfile=true; Persist Security Info=false;";

            //string? line;
            //int lineNumber = 0;
            //int lineProcessed = 0;

            var registrosPermitidos = new HashSet<string>
            {
                "0000",
                "0001",
                "0010",
                "L001",
                "L030",
                "L100", "L300",
                "P001", "P030", "P100", "P150",
            };




            // 1️⃣ marca o arquivo como "em processamento"
            //var file = await _operationFileRepository.GetByIdAsync(FileId)
            //    ?? throw new Exception("Arquivo não encontrado ");

            //file.Start();

            //_operationFileRepository.Update(file);
            //await _operationFileRepository.SaveChangesAsync();

            // 2️⃣ lê o arquivo do object storage
            var swTotal = Stopwatch.StartNew();
            var layouts = (await _layoutRepository.LoadAllAsync())
                  .ToDictionary(x => x.Registro, StringComparer.OrdinalIgnoreCase);

            await using var stream = await _objectStorage.OpenReadAsync($"{IdOp}", FileName);
            // 2. Cria o Channel — nasce e morre por processamento
            //var channel = Channel.CreateBounded<ParsedRow>(
            //    new BoundedChannelOptions(10_000)
            //    {
            //        FullMode = BoundedChannelFullMode.Wait,
            //        SingleWriter = true,
            //        SingleReader = true
            //    });

            //var parser = new SpedStreamParser(layouts, _writer);

            var channel = Channel.CreateBounded<ParsedRow>(
                new BoundedChannelOptions(40_000)
                {
                    FullMode = BoundedChannelFullMode.Wait, // parser espera se banco lento
                    SingleWriter = true,  // só um parser por arquivo
                    SingleReader = true   // só um writer consome
                });



            var parser = new SpedStreamParser(layouts, channel.Writer);
            //parser.ParseAsync(stream, FileId, IdOp, IdTenant, IdEmpresa, 10, competencia: null).Wait();


            await using var conn = new MySqlConnection(_connectionString);
            await conn.OpenAsync(cancellationToken);
            await using var writer = new SqlBulkWriter(conn, layouts, 10);
            //channel.Writer.Complete();

            Console.Write("comecando");
            var parseTask = ParseComFechamentoAsync(
              parser, stream, FileId, IdOp, IdTenant, IdEmpresa,
              10, channel.Writer, cancellationToken);




            //var parseTask = parser.ParseAsync(parser, stream, FileId, IdOp, IdTenant, IdEmpresa,
            //   competencia, channel.Writer, cancellationToken);

            var writeTask = writer.ConsumeAsync(channel.Reader, cancellationToken);

            // Aguarda ambos — se qualquer um lançar exceção, propaga
            await Task.WhenAll(parseTask, writeTask);







            swTotal.Stop();
            Console.WriteLine($"Tempo TOTAL ParseAsync: {swTotal.ElapsedMilliseconds} ms");
            Console.Write("terminou");
            //return;

            //using var reader = new StreamReader(stream, Encoding.GetEncoding("ISO-8859-1"));
            //stream.Position = 0;

            ////verfica se a competencia ja existe para esse cnpj
            //var primeiraLinha = await reader.ReadLineAsync();
            //if (string.IsNullOrWhiteSpace(primeiraLinha))
            //    throw new Exception("Não foi possível fazer a leitura da linha");

            //var competenciaString = ExtrairCompetencia(primeiraLinha);
            //var cnpjRaiz = ExtrairCnpjRaiz(primeiraLinha);
            //var campos0000 = primeiraLinha.Split('|');
            //var firstReg = campos0000[1];
            //lineProcessed++;
            //stack.Push(new StackItem { Reg = firstReg, Id = lineProcessed });
            //var strategyy = _ecfFactory.ObterPorReg(firstReg);

            //var entidadee = strategyy.Build(
            //    primeiraLinha,
            //    Id: lineProcessed,
            //    IdOp,
            //    IdTenant,
            //    IdEmpresa,
            //    IdPai: null,
            //    FileId,
            //    FileName,
            //    competenciaString
            //);

            //Add(entidadee, _buffer);

            //var competencia = DateTime.ParseExact(
            //    competenciaString,
            //    "yyyyMM",
            //    CultureInfo.InvariantCulture
            //);




            //var query = e_0000Repository.Query(o =>
            //       o.Cnpj.StartsWith(campos0000[4].Substring(0, 8)) &&
            // o.DtIni == ConverterData(campos0000, 10));

            //var listaExistentes = query.ToList();

            //if (listaExistentes.Any())
            //{
            //    if (overwrite == false)
            //        throw new Exception($"Já existe uma operação para o CNPJ raiz {cnpjRaiz} na competência {competencia}.");


            //    e_0000Repository.DeleteRange(listaExistentes);
            //    await e_0000Repository.SaveChangesAsync();
            //}

          

            //while ((line = await reader.ReadLineAsync()) != null)
            //{
            //    lineNumber++;


            //    if (string.IsNullOrWhiteSpace(line))
            //        continue;
            //    var campos = line.Split('|', StringSplitOptions.RemoveEmptyEntries);





            //    var reg = campos[0];
            //    if (!registrosPermitidos.Contains(reg))
            //        continue;

            //    lineProcessed++;
            //    if (!hierarchy.TryGetValue(reg, out var parentReg))
            //    {
            //        continue;
            //    }

            //    // 4️⃣ ajusta stack até encontrar o pai correto
            //    while (stack.Any() && stack.Peek().Reg != parentReg)
            //    {
            //        stack.Pop();
            //    }

            //    var pai = stack.Any() ? stack.Peek() : null;
            //    var idPai = pai?.Id;

            //    stack.Push(new StackItem
            //    {
            //        Reg = reg,
            //        Id = lineProcessed
            //    });

            //    try
            //    {
            //        var strategy = _ecfFactory.ObterPorReg(reg);

            //        var entidade = strategy.Build(
            //            line,
            //            Id: lineProcessed,
            //            IdOp,
            //            IdTenant,
            //            IdEmpresa,
            //            idPai.HasValue ? idPai.Value.GetHashCode() : null,
            //            FileId,          // fileId
            //            FileName,    // fileName
            //            competenciaString             // competencia (pode ser extraída da linha 0000 ou passada como parâmetro)  
            //        );
            //        Add(entidade, _buffer);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception($"Erro ao processar linha {lineNumber}: {ex.Message}", ex);
            //        continue;
            //    }


            //}
            //await _bulkInsertService.FlushAsync(_buffer, cancellationToken);

            //await Task.CompletedTask;
        }
        private static string ExtrairCompetencia(string linha)
        {
            var data = ExtrairDataInicial(linha);

            if (DateTime.TryParseExact(
                data,
                "ddMMyyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var dt))
            {
                return dt.ToString("yyyyMM");
            }

            return string.Empty;
        }


        private static async Task ParseComFechamentoAsync(
             SpedStreamParser parser,
             Stream stream,
             long fileId,
             long idOp,
             long idTenant,
             long idEmpresa,
             int layoutNumber,
             ChannelWriter<ParsedRow> channelWriter,
             CancellationToken cancellationToken)
        {
            try
            {
                await parser.ParseAsync(
                    stream, fileId, idOp, idTenant, idEmpresa,
                    layoutNumber);
            }
            finally
            {
                // Sempre fecha — mesmo se der exceção
                // Sem isso o ConsumeAsync fica preso esperando para sempre
                channelWriter.TryComplete();
            }
        }

        private static string ExtrairDataInicial(string linha)
        {
            // Campo 10 → data inicial (ddMMyyyy)
            var match = Regex.Match(linha, @"^\|0000\|(?:[^|]*\|){9}(\d{8})\|");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }



        private static string ExtrairCnpjRaiz(string linha)
        {
            var campos = linha.Split('|', StringSplitOptions.RemoveEmptyEntries);

            if (campos.Length > 3 && campos[0] == "0000")
                return campos[3].Substring(0, 8);

            return string.Empty;
        }
        public void Add(EcfBase entidade, Dictionary<Type, IList<EcfBase>> buffer)
        {
            var type = entidade.GetType();

            if (!buffer.TryGetValue(type, out var list))
            {
                list = new List<EcfBase>(capacity: 1024);
                buffer[type] = list;
            }

            list.Add(entidade);
        }

        private static string ExtractReg(string line)
        {
            // |0000|LECF|...
            int firstPipe = line.IndexOf('|');
            if (firstPipe < 0)
                throw new FormatException("Linha inválida: pipe inicial não encontrado.");

            int secondPipe = line.IndexOf('|', firstPipe + 1);
            if (secondPipe < 0)
                throw new FormatException("Linha inválida: segundo pipe não encontrado.");

            return line.Substring(firstPipe + 1, secondPipe - firstPipe - 1);
        }
        public Dictionary<string, string?> Load(IEnumerable<Relationship> rels)
          => rels.ToDictionary(
              r => r.Reg,
              r => string.IsNullOrWhiteSpace(r.RegPai)
                  ? null
                  : r.RegPai
          );

        private static DateTime? ConverterData(string[] campos, int index)
        {
            if (campos.Length <= index || string.IsNullOrWhiteSpace(campos[index]))
                return null;

            if (DateTime.TryParseExact(
                campos[index],
                "ddMMyyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var data))
            {
                return data;
            }

            return null;
        }




        public static Dictionary<string, string?> Create()
        {
            return new Dictionary<string, string?>
            {
                // =========================
                // BLOCO 0 — ABERTURA
                // =========================
                ["0000"] = null,
                ["0001"] = "0000",

                ["0010"] = "0001",
                ["0020"] = "0001",
                ["0030"] = "0001",
                ["0035"] = "0001",
                ["0930"] = "0001",
                ["0990"] = "0001",

                // =========================
                // BLOCO C — LIVRO CAIXA
                // =========================
                ["C001"] = "0001",
                ["C010"] = "C001",
                ["C040"] = "C001",
                ["C050"] = "C001",
                ["C051"] = "C050",
                ["C052"] = "C050",
                ["C053"] = "C050",
                ["C990"] = "C001",

                // =========================
                // BLOCO E — APURAÇÃO
                // =========================
                ["E001"] = "0001",
                ["E010"] = "E001",
                ["E020"] = "E010",
                ["E030"] = "E010",
                ["E040"] = "E010",
                ["E990"] = "E001",

                // =========================
                // BLOCO J — DEMONSTRAÇÕES
                // =========================
                ["J001"] = "0001",
                ["J005"] = "J001",
                ["J050"] = "J005",
                ["J051"] = "J005",
                ["J100"] = "J001",
                ["J150"] = "J001",
                ["J900"] = "J001",
                ["J930"] = "J001",
                ["J990"] = "J001",

                // =========================
                // BLOCO K — SALDOS
                // =========================
                ["K001"] = "0001",
                ["K030"] = "K001",
                ["K031"] = "K030",
                ["K032"] = "K030",
                ["K990"] = "K001",

                // =========================
                // BLOCO L — LALUR / LACS
                // =========================
                ["L001"] = "0000",
                ["L030"] = "L001",
                ["L100"] = "L030",
                ["L200"] = "L030",
                ["L210"] = "L030",
                ["L300"] = "L030",
                ["L990"] = "0000",

                // =========================
                // BLOCO P — PREÇOS DE TRANSFERÊNCIA
                // =========================
                ["P001"] = "0000",
                ["P030"] = "P001",
                ["P100"] = "P030",
                ["P150"] = "P030",
                ["P200"] = "P030",
                ["P300"] = "P030",
                ["P400"] = "P030",
                ["P990"] = "0000",

                // =========================
                // BLOCO 9 — ENCERRAMENTO
                // =========================
                ["9001"] = "0001",
                ["9900"] = "9001",
                ["9990"] = "9001",
                ["9999"] = "0000",
            };
        }
    }
}
