# Capag FCO — parâmetros por empresa

Persistência de **exceções L100/L300** no serviço CAPAG / banco `gsaas`. O catálogo universal (~732 + ~386 contas) permanece no client (`parametros-l100.json` / `parametros-l300.json`). Empresa nova = zero linhas = 100% default.

## Path (client-app → `REACT_APP_API_URL` / proxy BFF)

```
GET|PUT  /api/empresas/{idEmpresa}/capag-fco/parametros
```

O path segue o padrão de `capag-simples`.

Headers obrigatórios (igual às demais rotas autenticadas):

- `Authorization: Bearer {tokenCapag}`
- `X-Tenant-Id: {id_tenant}`

`idEmpresa` = `id_empresa` da tabela `empresas` (Capag). **Não** usar `company_id` do gmsystem. O campo JSON `companyId` da resposta é esse mesmo `id_empresa`.

Mutações (`PUT`) exigem papel `Admin` ou `editor`.

## Contrato JSON

Respostas e bodies em **camelCase** (`companyId`, `excecoesL100`, `versaoBase`, etc.), via `[JsonPropertyName]`.

### GET

Sem registro (empresa usa 100% o catálogo):

```json
{
  "companyId": 123,
  "excecoesL100": {},
  "excecoesL300": {},
  "baseVersaoHash": null,
  "updatedAt": null,
  "contasPersonalizadasL100": 0,
  "contasPersonalizadasL300": 0
}
```

Com exceções:

```json
{
  "companyId": 123,
  "excecoesL100": { "1.02.03.01.30": { "grupoDfc": "SEM EFEITO DE CAIXA", "trat": "T9" } },
  "excecoesL300": {},
  "baseVersaoHash": "a1b2c3d4e5f67890",
  "updatedAt": "2026-08-26T21:00:00",
  "contasPersonalizadasL100": 1,
  "contasPersonalizadasL300": 0
}
```

Campos de cada exceção (só os que diferem do catálogo): `grupoDfc`, `trat`, `acao`, `parConta`, `revisar`, `justificativa`.

### PUT

Substitui **um bloco** por vez. `excecoes` vazio limpa o bloco (volta ao default). O outro bloco não é alterado.

```json
{
  "bloco": "l100",
  "excecoes": { "1.02.03.01.30": { "trat": "T9" } },
  "versaoBase": "a1b2c3d4e5f67890"
}
```

`bloco` aceita `l100` ou `l300` (case-insensitive). `versaoBase` tem no máximo 16 caracteres e atualiza `base_versao_hash`.

A resposta do PUT é o mesmo payload do GET, já com o bloco persistido.

Se o client (`fcoParametrosService.ts`) usa outro path, criar proxy BFF em `client-app/src/pages/api/capag/fco/parametros.ts` mapeando para `/api/empresas/{idEmpresa}/capag-fco/parametros`.

## DDL

Arquivo: `Database/Migrations/20260827_fco_parametro_empresa.sql`

Tabela:

| Tabela | Papel |
|--------|--------|
| `fco_parametro_empresa` | 1 registro por `id_empresa`; JSON só com exceções L100 e L300 |

Aplicar em **dev → homolog → prod** antes de expor os endpoints.

## Smoke test

1. `GET .../parametros` em empresa sem registro → mapas vazios e contagens 0
2. `PUT .../parametros` com `bloco: l100` e uma conta
3. `GET .../parametros` → `excecoesL100` preenchido, `contasPersonalizadasL100 = 1`, L300 vazio
4. `PUT .../parametros` com `bloco: l300` e `excecoes: {}` → L100 permanece, L300 vazio
5. Empresa inexistente / de outro tenant → 404
