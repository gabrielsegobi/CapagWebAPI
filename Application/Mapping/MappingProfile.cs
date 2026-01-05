using AutoMapper;
using Domain.Contracts.AnalisesICP;
using Domain.Contracts.DemonstrativosContabeis;
using Domain.Contracts.DocumentsLayouts;
using Domain.Contracts.Empresas;
using Domain.Contracts.ExtractionRules;
using Domain.Contracts.ICPLimits;
using Domain.Contracts.Indicadores;
using Domain.Contracts.ModelosIndicesICP;
using Domain.Contracts.ProcessLog;
using Domain.Contracts.RefreshTokens;
using Domain.Contracts.RegDarf;
using Domain.Contracts.RegDirfTerceiros;
using Domain.Contracts.RegimesTributarios;
using Domain.Contracts.RegIrpf;
using Domain.Contracts.RegsDctf;
using Domain.Contracts.RegsPgdasd;
using Domain.Contracts.ResultadosIndicesICP;
using Domain.Contracts.Tenants;
using Domain.Contracts.TipoGrupos;
using Domain.Contracts.Usuarios;
using Domain.Contracts.UsuarioTenant;
using Domain.Contracts.ValoresAnuais;
using Domain.Contracts.Views;
using Domain.Entities;
using Domain.Entities.Views;
using Domain.Resources;
using Infrastructure.Helpers;
using Infrastructure.Security;

namespace Application.Mapping
{
    public class MappingProfile : Profile

    {
        public MappingProfile()
        {
            #region Emrpesas
            CreateMap<Empresa, EmpresaDto>().ReverseMap();

            CreateMap<CreateEmpresaRequest, Empresa>()
                  .ForMember(e => e.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(e => e.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(e => e.DadosProcessados, opt => opt.MapFrom(src => false));
            //.ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => Cnpj.Criar(src.Cnpj)));

            CreateMap<UpdateEmpresaRequest, Empresa>()
                  .ForMember(e => e.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            //.ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => Cnpj.Criar(src.Cnpj)));

            #endregion

            #region Usuarios
            CreateMap<Usuario, UsuarioDto>().ReverseMap();
            CreateMap<CreateUsuarioRequest, Usuario>()
                  .ForMember(u => u.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(u => u.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(u => u.Ativo, opt => opt.MapFrom(src => true));
            CreateMap<UpdateUsuarioRequest, Usuario>();
            #endregion


            #region Tenants
            CreateMap<Tenant, TenantDto>().ReverseMap();

            CreateMap<CreateTenantRequest, Tenant>()
                  .ForMember(u => u.DataCriacao, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));

            CreateMap<UpdateTenantRequest, Tenant>();
            #endregion

            #region UsuarioTenant
            CreateMap<UsuarioTenant, UsuarioTenantDto>().ReverseMap();

            CreateMap<CreateUsuarioTenantRequest, UsuarioTenant>()
                  .ForMember(u => u.DataVinculo, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));

            CreateMap<UpdateUsuarioTenantRequest, UsuarioTenant>();
            #endregion

            #region DemonstrativosContabeis
            CreateMap<CreateBalancoRequest, DemonstrativoContabil>()
               .ForMember(u => u.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
               .ForMember(u => u.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));

            CreateMap<CreateDRERequest, DemonstrativoContabil>()
                 .ForMember(u => u.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                 .ForMember(u => u.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            #endregion

            #region RegimesTributarios
            CreateMap<RegimeTributario, RegimeTributarioDto>().ReverseMap();
            CreateMap<CreateRegimeTributarioRequest, RegimeTributario>()
                 .ForMember(r => r.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                 .ForMember(r => r.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            #endregion

            #region Indicadores
            CreateMap<CreateIndicadorRequest, Indicador>()
                  .ForMember(i => i.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(i => i.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));

            CreateMap<Indicador, IndicadorDto>()
                  .ForMember(dto => dto.ValoresAnuais, opt => opt.MapFrom(src => src.ValoresAnuais))
                  .ForMember(dto => dto.Grupo, opt => opt.MapFrom(src => IndicadorGrupoMapper.ObterGrupo(src.Nome)))
                  .ForMember(dto => dto.GrupoDescricao, opt => opt.MapFrom(src => IndicadorGrupoMapper.ObterDescricaoGrupo(src.Nome)))
                  .ForMember(dto => dto.DescricaoIndicador, opt => opt.MapFrom(src => IndicadorDescricaoMapper.ObterDescricao(src.Nome)))

                  .ReverseMap();
            #endregion

            #region ValoresAnuais
            CreateMap<CreateValorAnualRequest, ValorAnual>()
                  .ForMember(va => va.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(va => va.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));

            CreateMap<ValorAnual, ValorAnualDto>().ReverseMap();
            #endregion

            #region ModelosIndicesICP
            CreateMap<ModeloIndiceICP, ModeloIndiceICPDto>()
                  .ForMember(dto => dto.Resultados, opt => opt.MapFrom(src => src.Resultados)).ReverseMap();

            #endregion

            #region  ResultadosIndicesICP
            CreateMap<ResultadoIndiceICP, ResultadoIndiceICPDto>().ReverseMap();
            #endregion

            #region  AnalisesICP
            CreateMap<AnaliseICP, AnaliseICPDto>().ReverseMap();
            CreateMap<CreateAnaliseICPRequest, AnaliseICP>()
                  .ForMember(a => a.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(a => a.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            #endregion

            #region ResultadosIndicesICP
            CreateMap<CreateResultadoIndiceICPRequest, ResultadoIndiceICP>()
                  .ForMember(ri => ri.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(ri => ri.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            #endregion

            #region RefreshTokens
            CreateMap<CreateRefreshTokenRequest, RefreshToken>()
                  .ForMember(rt => rt.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(rt => rt.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(rt => rt.ExpiraEm, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow().AddDays(1)))
                  .ForMember(rt => rt.Token, opt => opt.MapFrom(src => RandomGenerator.GenerateDefault()));
            #endregion

            #region ICPLimits
            CreateMap<ICPLimit, ICPLimitDto>().ReverseMap();
            CreateMap<UpdateICPLimitRequest, ICPLimit>()
                  .ForMember(il => il.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()))
                  .ForMember(il => il.UpdatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            #endregion


            #region ProcessLog
            CreateMap<ProcessLog, ProcessLogDto>().ReverseMap();
            CreateMap<CreateProcessLogRequest, ProcessLog>()
                  .ForMember(pl => pl.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            #endregion

            #region ProcessLog
            CreateMap<ProcessLog, ProcessLogDto>().ReverseMap();
            CreateMap<CreateProcessLogRequest, ProcessLog>()
                  .ForMember(pl => pl.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            #endregion

            #region DocumentLayouts
            CreateMap<DocumentLayout, DocumentLayoutDto>().ReverseMap()
              .ForMember(dto => dto.ExtractionRules, opt => opt.MapFrom(src => src.ExtractionRules)).ReverseMap();
            CreateMap<CreateDocumentLayoutRequest, DocumentLayout>()
                  .ForMember(pl => pl.CreatedAt, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            CreateMap<UpdateDocumentLayoutRequest, DocumentLayout>();
            #endregion

            #region ExtractionRules
            CreateMap<ExtractionRule, ExtractionRuleDto>().ReverseMap();
            CreateMap<CreateExtractionRuleRequest, ExtractionRule>();
            CreateMap<UpdateExtractionRuleRequest, ExtractionRule>();
            #endregion


            #region RegDarf
            CreateMap<RegDarfs, RegDarfDto>().ReverseMap();
            CreateMap<CreateRegDarfRequest, RegDarfs>()
                  .ForMember(a => a.DataProcessamento, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            CreateMap<RegDarfItemRequest, RegDarfs>()
                .ForMember(a => a.DataProcessamento, opt => opt.MapFrom(src => DateTimeHelper.GetDateTimeNow()));
            CreateMap<UpdateRegDarfRequest, RegDarfs>();
            #endregion

            #region RegDirfTerceiro
            CreateMap<RegDirfTerceiro, RegDirfTerceiroDto>().ReverseMap();
            CreateMap<CreateRegDirfTerceiroRequest, RegDirfTerceiro>();
            CreateMap<RegDirfTerceiroItemRequest, RegDirfTerceiro>();
            CreateMap<UpdateRegDirfTerceiroRequest, RegDirfTerceiro>();
            #endregion

            #region RegDctf
            CreateMap<RegDctf, RegDctfDto>().ReverseMap();
            CreateMap<CreateRegDctfRequest, RegDctf>();
            CreateMap<RegDctfItemRequest, RegDctf>();
            CreateMap<UpdateRegDctfRequest, RegDctf>();
            #endregion

            #region RegIrpf
            CreateMap<RegIrpf, RegIrpfDto>().ReverseMap();
            CreateMap<CreateRegIrpfRequest, RegIrpf>();
            CreateMap<RegIrpfItemRequest, RegIrpf>();
            #endregion

            #region TipoGrupo
            CreateMap<TipoGrupo, TipoGrupoDto>().ReverseMap();
            CreateMap<RegIrpf, PfCalculoDto>().ReverseMap();
            #endregion

            #region RegPgdasd
            CreateMap<RegPgdasd, RegPgdasdDto>().ReverseMap();
            CreateMap<CreateRegPgdasdRequest, RegPgdasd>();
            CreateMap<RegPgdasdItemRequest, RegPgdasd>();
            #endregion

            #region View
            CreateMap<BalancoPatrimonialVw, BPViewDto>();
            CreateMap<DashboardEmpresaAnualVw, DEAViewDto>();
            CreateMap<DREVw, DREViewDto>();
            CreateMap<UsuariosAcessosVw, UAViewDto>();
            #endregion
        }
    }
}
