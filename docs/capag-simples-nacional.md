# Capag Simples Nacional (gsaas)

Persistência de **declaração Simples Nacional** e **valores manuais de DRE/Balanço** no serviço CAPAG / banco `gsaas`.

## Path final (client-app → `apiCapag` / `/api/proxy_capag`)

```
GET|PUT|DELETE  /api/empresas/{idEmpresa}/capag-simples/declaration
GET|PUT         /api/empresas/{idEmpresa}/capag-simples/demonstrative-values
```

Headers obrigatórios (igual às demais rotas autenticadas):

- `Authorization: Bearer {tokenCapag}`
- `X-Tenant-Id: {id_tenant}`

`idEmpresa` = `id_empresa` da tabela `empresas` (Capag). **Não** usar `company_id` do gmsystem.

Mutações (`PUT` / `DELETE`) exigem papel `Admin` ou `editor`.

## Contrato JSON

Respostas e bodies em **camelCase** (`hasDeclaration`, `declarationKind`, `exerciseYears`, `balanceSheet`, `porCodigo`, etc.), via `[JsonPropertyName]`, para manter compatibilidade com o client atual. O restante da API Capag costuma usar snake_case; este recurso é exceção documentada.

### Declaração

`GET` sem declaração:

```json
{ "hasDeclaration": false }
```

`PUT` body:

```json
{
  "declarationKind": "SIMPLES_EXERCISE_YEARS",
  "exerciseYears": [2022, 2024]
}
```

ou

```json
{
  "declarationKind": "NO_NATIONAL_SIMPLE_STRICT",
  "exerciseYears": []
}
```

### Valores manuais

`GET ?kind=DRE` (opcional; repetir `kind` para filtrar). Sem `kind` retorna `dre` e `balanceSheet`.

`PUT` body (replace do kind inteiro; `porCodigo` vazio limpa):

```json
{
  "demonstrativeKind": "DRE",
  "porCodigo": {
    "3.01.01": { "2024": 1000.5 }
  }
}
```

## DDL

Arquivo: `Database/Migrations/20260805_simples_nacional_gsaas.sql`

Tabelas:

| Tabela | Papel |
|--------|--------|
| `simples_declaration` | 1 declaração por `id_empresa` |
| `simples_exercise_year` | Anos (cascade delete via `id_simples_declaration`) |
| `manual_demonstrative_value` | Valores manuais por conta × ano × kind |

Aplicar em **dev → homolog → prod** antes de expor os endpoints.

## Smoke test

1. `GET .../declaration` → `{ "hasDeclaration": false }`
2. `PUT .../declaration` com `SIMPLES_EXERCISE_YEARS` + anos
3. `GET .../declaration` → declaração preenchida
4. `PUT .../demonstrative-values` kind `DRE`
5. `GET .../demonstrative-values?kind=DRE`
6. `DELETE .../declaration` → `200`; `GET` de novo → vazio

## Fora de escopo

- Ajuste do frontend gmaster-system
- Drop das tabelas no gmsystem
- Migração de dados gmsystem → gsaas (mapear `company_id` → `id_empresa` se houver dado em produção)
