using AutoMapper;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Contracts.Json;
using Domain.Contracts.RegimesTributarios;
using Domain.Entities;
using Infrastructure.Interface;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.RegularExpressions;


namespace Application
{
    public class IntegracaoDemonstrativosService : IIntegracaoDemonstrativosService
    {
        private readonly IApiDadosService _apiService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly string _baseUrl;

        public IntegracaoDemonstrativosService(IApiDadosService apiService, IMapper mapper, IConfiguration configuration)
        {
            _apiService = apiService;
            _mapper = mapper;
            _configuration = configuration;
            _baseUrl = _configuration["ApiGmaster:BaseUrl"] ?? throw new ArgumentNullException("ApiGmaster:BaseUrl");

        }
        
        public async Task<List<CreateDRERequest>> ObterDreAsync(Empresa empresa, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/dre?ano=2021,2022,2023,2024&cnpj={empresa.Cnpj}";
            var json = await _apiService.ObterDadosApiAsync<DREJson>(url, cancellationToken);


            var mapped = json.Data.Select(d => new CreateDRERequest
            {
                IdEmpresa = empresa.IdEmpresa,
                IdTenant = empresa.IdTenant,
                Ano = int.TryParse(d.ANO, out var ano) ? ano : 0,
                Codigo = d.CODIGO,
                Descricao = d.DESCRICAO,
                Tipo = string.IsNullOrEmpty(d.TIPO) ? '\0' : d.TIPO[0],
                Nivel = byte.TryParse(d.NIVEL, out var nivel) ? nivel : (byte)0,
                TipoTrib = d.TIPO_TRIB,
                PerApur = d.PER_APUR,
                DtIni = DateTime.TryParse(d.DT_INI, out var dtIni) ? DateOnly.FromDateTime(dtIni) : DateOnly.MinValue,
                DtIniApur = DateTime.TryParse(d.DT_INI_APUR, out var dtIniApur) ? DateOnly.FromDateTime(dtIniApur) : DateOnly.MinValue,
                DtFinApur = DateTime.TryParse(d.DT_FIN_APUR, out var dtFinApur) ? DateOnly.FromDateTime(dtFinApur) : DateOnly.MinValue,
                ValCtaRefFin = decimal.TryParse(d.VALOR, NumberStyles.Any, CultureInfo.GetCultureInfo("en-US"), out var valor) ? valor : 0,
                IndValCtaRefFin = d.IND_VALOR
            }).ToList();

            return mapped;
        }

        public async Task<List<CreateBalancoRequest>> ObterBalancoAsync(Empresa empresa, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/balanco?ano=2021,2022,2023,2024&cnpj={empresa.Cnpj}";
            var json = await _apiService.ObterDadosApiAsync<BalancoJson>(url, cancellationToken);
            var culture = CultureInfo.GetCultureInfo("en-US");

            var mapped = json.Data.Select(d => new CreateBalancoRequest
            {
                IdEmpresa = empresa.IdEmpresa,
                IdTenant = empresa.IdTenant,
                Ano = int.Parse(d.ANO),
                Codigo = d.CODIGO,
                Descricao = d.DESCRICAO,
                Tipo = d.TIPO.FirstOrDefault(),
                Nivel = byte.Parse(d.NIVEL),
                TipoTrib = d.TIPO_TRIB,
                PerApur = d.PER_APUR,
                DtIni = DateOnly.FromDateTime(DateTime.Parse(d.DT_INI)),
                DtIniApur = DateOnly.FromDateTime(DateTime.Parse(d.DT_INI_APUR)),
                DtFinApur = DateOnly.FromDateTime(DateTime.Parse(d.DT_FIN_APUR)),

                ValCtaRefIni = decimal.TryParse(d.VAL_CTA_REF_INI, NumberStyles.Any, culture, out var ini) ? ini : 0,
                ValCtaRefDeb = decimal.TryParse(d.VAL_CTA_REF_DEB, NumberStyles.Any, culture, out var deb) ? deb : 0,
                ValCtaRefCred = decimal.TryParse(d.VAL_CTA_REF_CRED, NumberStyles.Any, culture, out var cred) ? cred : 0,
                ValCtaRefFin = decimal.TryParse(d.VAL_CTA_REF_FIN, NumberStyles.Any, culture, out var fin) ? fin : 0,
            }).ToList();

            return mapped;
        }

        public async Task<List<CreateRegimeTributarioRequest>> ObterTributacoesAsync(Empresa empresa, CancellationToken cancellationToken)
        {
            var url = $"{_baseUrl}/tributacao?cnpj={empresa.Cnpj}";
            var json = await _apiService.ObterDadosApiAsync<RegimeTributarioJson>(url, cancellationToken);

            var mapped = json.Data.Select(d => new CreateRegimeTributarioRequest
            {
                IdEmpresa = empresa.IdEmpresa,
                IdTenant = empresa.IdTenant,
                Ano = int.Parse(d.ANO),
                DtIni = DateOnly.FromDateTime(DateTime.Parse(d.DT_INI)),
                RaizCnpj = d.RAIZ,
                FormaApurCompleta = d.FORMA_APUR_COMPLETA,
                FormaTribCompleta = d.FORMA_TRIB_COMPLETA,
            }).ToList();

            return mapped;
        }

        private void InjetarEmpresa<T>(List<T> lista, Empresa empresa)
        {
            foreach (var item in lista)
            {
                var propIdEmp = item?.GetType().GetProperty("IdEmpresa");
                var propIdTenant = item?.GetType().GetProperty("IdTenant");

                if (propIdEmp != null)
                    propIdEmp.SetValue(item, empresa.IdEmpresa);
                if (propIdTenant != null)
                    propIdTenant.SetValue(item, empresa.IdTenant);
            }
        }
    }
}
