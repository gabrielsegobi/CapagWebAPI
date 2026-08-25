# Integração frontend — Balanço Patrimonial

Documento para o projeto de **frontend** (repositório separado) que consome esta API. Descreve como exibir o balanço **sem alterar** as regras contábeis já implementadas no backend.

Para a grade de indicadores, ver `integracao-frontend-indicadores.md`.

---

## Resumo

| O que exibir na coluna de um **ano** | Campo da API |
|--------------------------------------|--------------|
| Saldo do exercício (padrão na tela)  | `valor` ou `val_cta_ref_fin` |
| Saldo de abertura (uso especial)     | `valor_inicial` ou `val_cta_ref_ini` |

**Não** usar `valor_inicial` / `val_cta_ref_ini` como valor principal da coluna anual do balanço.

**Não** reutilizar o saldo final do ano anterior como valor do ano corrente.

---

## Endpoints

### Tela de Balanço Patrimonial (principal)

```
GET /api/Views/balanco-patrimonial?IdEmpresa={id}&Ano={ano opcional}&Page=1&PageSize=500
Headers: Authorization, X-Tenant-Id
```

Retorna `PagedApiResponse<BPViewDto>`. Cada item é **uma conta em um ano** (`codigo` + `ano`).

Campos relevantes do `BPViewDto`:

| Campo JSON | Significado |
|------------|-------------|
| `codigo` | Código contábil ECD (ex.: `1.01.03`) |
| `ano` | Exercício |
| `per_apur` | Período (`A00` = anual; `T01`…`T04` = trimestral) |
| `val_cta_ref_ini` | Saldo de **abertura** do exercício |
| `val_cta_ref_fin` | Saldo de **fechamento** do exercício |
| `valor` | Alias de `val_cta_ref_fin` — **preferir na UI** |
| `valor_inicial` | Alias de `val_cta_ref_ini` |
| `tipo` | `S` sintética, `P` analítica |
| `tipo_trib` | Ex.: `Simples Nacional`, `Lucro Real` |

A view `vw_balanco_patrimonial` só inclui registros com `val_cta_ref_ini IS NOT NULL` e `deleted_at IS NULL`.

**Paginação:** quando `IdEmpresa` é informado, o backend eleva o `PageSize` para até **500** se vier o padrão (10). Mesmo assim, para empresas com muitos anos/contas, usar `PageSize=500` e percorrer todas as páginas até `total` esgotado.

**Ordenação:** respostas vêm ordenadas por `codigo`, depois `ano` (ascendente).

### Indicadores / motor de fórmulas (não usar direto na grade do balanço)

```
GET /api/Empresas/demonstrativos-contabeis?idEmpresa={id}&ano=true
```

Retorna valores **consolidados** por `codigo` e `ano`:

- `{codigo}` → saldo **final** do exercício (balanço anual `A00` ou `T04` trimestral).
- `{codigo}[I]` → saldo **inicial** (abertura `A00`, ou `T04` do ano anterior no regime trimestral).

Esse endpoint segue `GetDClByAnoAndCodigoHandler` e alimenta **indicadores** (`CalcIndicadoresHandler`). A grade do balanço deve usar **`balanco-patrimonial`**, não este endpoint, para manter `ini`/`fin` separados na tela.

---

## Regras contábeis do backend (não reimplementar no front)

### 1. Dois saldos por conta (layout ECD/SPED)

Toda linha de **balanço** possui:

- **`val_cta_ref_ini`** — posição em 01/01 do exercício (abertura).
- **`val_cta_ref_fin`** — posição em 31/12 do exercício (fechamento).

Na **coluna de cada ano** na UI, o valor exibido deve ser o **fechamento** (`valor` / `val_cta_ref_fin`).

O saldo inicial serve para:

- colunas de abertura (se existirem),
- indicadores com sufixo `[I]` (ex.: estoque médio),
- conferência — **não** substitui o final na coluna do ano.

### 2. Construção via `/construir` (Simples Nacional / DEFIS)

`POST /api/demonstrativos/construir` lê a DEFIS e grava:

| Conta | `val_cta_ref_ini` | `val_cta_ref_fin` |
|-------|-------------------|-------------------|
| `1.01.01` | Saldo caixa/banco **no início** | Saldo caixa/banco **no final** |
| `1.01.03` | Estoque **inicial** | Estoque **final** |
| `1.01`, `1` (sintéticas) | Soma dos iniciais | Soma dos finais |

Exemplo DEFIS 2024 (empresa 199):

| DEFIS | Valor | Gravado em |
|-------|-------|------------|
| Caixa início / final | 17.182 / 26.980 | `1.01.01` ini / fin |
| Estoque inicial / final | 32.145 / 58.926 | `1.01.03` ini / fin |
| Ativo circulante | — | `1.01` fin = **85.906** (26.980 + 58.926) |

O `/construir` **não troca** inicial com final. Se a tela mostra 32.145 ou 322.578 na coluna 2024 de estoques, o erro está na **montagem da grade**, não na API.

### 3. Regime trimestral (Lucro Real / ECF) vs anual (`A00`)

| Tipo | `per_apur` | Valor na coluna do ano (`valor`) | Valor `[I]` (indicadores) |
|------|------------|----------------------------------|---------------------------|
| Balanço trimestral | `T01`…`T04` | Saldo em **T04** (fim do ano) | `T04` do **ano anterior** |
| Balanço anual | `A00` | `val_cta_ref_fin` do `A00` | `val_cta_ref_ini` do `A00` |
| DRE | qualquer | Soma dos trimestres (fluxo) | `val_cta_ref_ini` = null |

**Balanço = posição (estoque).** Não somar `T01+T02+T03+T04` na grade — isso é regra de **DRE** (fluxo), não de BP.

### 4. DRE vs Balanço

Linhas de **DRE** (`codigo` começando com `3`) têm `val_cta_ref_ini = null` e valor apenas em `val_cta_ref_fin` (fluxo do período). Não misturar lógica de DRE na tela de balanço.

---

## Como montar a grade no frontend

### Chave de agrupamento

Sempre indexar por **`(codigo, ano)`**, nunca só por `descricao` ou só por `codigo`.

```ts
// Exemplo
const mapa = new Map<string, BPViewDto>();
for (const row of todasAsPaginas) {
  mapa.set(`${row.codigo}|${row.ano}`, row);
}

function saldoExercicio(codigo: string, ano: number): number {
  const row = mapa.get(`${codigo}|${ano}`);
  return row?.valor ?? row?.val_cta_ref_fin ?? 0;
}
```

### Preenchimento das colunas por ano

Para cada ano selecionado pelo usuário e cada `codigo` da árvore:

```
célula[ano] = saldoExercicio(codigo, ano)  // usa val_cta_ref_fin
```

### Erros comuns (causam valores “trocados” ou de outro ano)

| Erro | Sintoma | Correção |
|------|---------|----------|
| Usar `val_cta_ref_ini` na coluna do ano | Mostra abertura (ex.: 32.145 em vez de 58.926) | Usar `valor` / `val_cta_ref_fin` |
| Usar saldo final do **ano anterior** | Coluna 2024 mostra 322.578 (fin de 2023) | Buscar registro com `ano === 2024` |
| Agrupar só por `descricao` ("ESTOQUES") | Ano errado quando há homônimos | Agrupar por `codigo` + `ano` |
| Paginação incompleta (10 itens) | Contas ausentes; UI reaproveita valor antigo | `PageSize=500` + todas as páginas |
| Somar trimestres no BP | Total inflado (ex.: contas a receber) | Usar `T04` ou `A00`, não soma |
| Aplicar regra `[I]` (T04 anterior) em `A00` Simples Nacional | Abertura errada na grade | `[I]` só no motor de indicadores |

### Conferência rápida (soma do ativo circulante)

Para contas vindas do `/construir` (Simples Nacional), em um mesmo ano:

```
val_cta_ref_fin("1.01.01") + val_cta_ref_fin("1.01.03") ≈ val_cta_ref_fin("1.01")
```

Se a soma das filhas não bater com o sintético, a UI está lendo campo ou ano incorreto.

---

## Exemplo real — empresa 199, ano 2024

API `balanco-patrimonial` (valores corretos):

| codigo | ano | valor_inicial | valor (fechamento) |
|--------|-----|---------------|---------------------|
| `1` | 2024 | 49.327 | **85.906** |
| `1.01.01` | 2024 | 17.182 | **26.980** |
| `1.01.03` | 2024 | 32.145 | **58.926** |
| `1.01.03` | 2023 | 335.287 | **322.578** |

Na coluna **2024**, estoques deve mostrar **58.926**, não 322.578 (que é fechamento de **2023**).

---

## Checklist para correção no frontend

- [ ] Fonte da grade: `GET /api/Views/balanco-patrimonial`
- [ ] Chave: `(codigo, ano)`
- [ ] Valor da célula: `valor` ou `val_cta_ref_fin`
- [ ] Carregar todas as páginas (`PageSize` ≥ 500)
- [ ] Não propagar saldo de um ano para a coluna de outro
- [ ] Não somar trimestres em contas de balanço
- [ ] Manter `valor_inicial` apenas se houver coluna de abertura ou debug
- [ ] Indicadores: continuar usando `demonstrativos-contabeis?ano=true` com `{codigo}` e `{codigo}[I]` — regras já centralizadas no backend

---

## DRE — consolidação anual e fonte única de verdade

### Regra de consolidação

O Capag usa `GetDClByAnoAndCodigoHandler` (`Ano=true`, sem `SomarPeriodosNoAno`) como fonte única de consolidação anual por código.

| Tipo de conta | Consolidação anual |
|---------------|--------------------|
| **DRE trimestral** (T01…T04) | Soma por período via `ValorParaFormula` (sinal econômico C→+, D→−) |
| **DRE anual** (A00) | `ValorParaFormula` de `val_cta_ref_fin` |
| **BP trimestral** | Fechamento = `T04` via `ValorParaFormula`; `[I]` = mesmo do `T04` do ano anterior |
| **BP anual** | Fechamento/`[I]` do `A00` via `ValorParaFormula` |

**Regra das fórmulas:** igual ao plano de contas — **crédito (azul) → positivo**, **débito → negativo**. Não inverter o Ativo de novo (o D/C do balanço já vem invertido em relação à DRE). Exceção: `3.01.01` e filhos em magnitude. **PMP**, **Ciclo Financeiro** e Cobertura de Juros (ICP) usam magnitude em todos os códigos da equação. **ROE** com PL (`2.03`) ≤ 0: `valor` nulo e `alerta`/`mensagem` = `"Não Analisar: Informação Comprometida"`. Demais indicadores calculam com o PL negativo.

**Exemplo — código `3.01.01` (Resultado Operacional / EBIT), sempre magnitude:**

| Período | Magnitude | Indicador | Efeito (Capag) |
|---------|-----------|-----------|----------------|
| T01 | 84.299,70 | C | +84.299,70 |
| T02 | 40.907,90 | C | +40.907,90 |
| T03 | 91.787,66 | D | +91.787,66 |
| T04 | 33.724,67 | C | +33.724,67 |
| **Total** | | | **250.719,93** |

Resultado líquido (`3`) com ind `D` entra **negativo** nas margens/ROA/ROE. Contas `3.01.01*` entram em magnitude (não são excluídas da DRE).

### Alinhamento com `dre-analise`

Os indicadores Capag usam a consolidação de `GetDClByAnoAndCodigoHandler` com sinal econômico. Exceções de magnitude (ciclo/PMR/CJ) são aplicadas na avaliação da fórmula, não na consolidação.

### Endpoint de consolidados anuais (conferência)

Para comparar o valor anual que o motor de indicadores enxerga:

```
GET /api/Empresas/demonstrativos-contabeis?idEmpresa={id}&ano=true
Headers: Authorization, X-Tenant-Id
```

Retorna lista de `{ codigo, ano, valor }` onde:
- `{codigo}` → valor para fórmula (C/azul → +; D → −; magnitude em `3.01.01*`)
- `{codigo}[I]` → saldo inicial com a mesma política

Esse endpoint é a mesma consolidação usada por `CalcIndicadoresHandler`. Usar para auditoria/debug — a grade detalhada de DRE continua em `GET /api/Views/dre`.

---

## Referências no código (CapagWebAPI)

| Responsabilidade | Arquivo |
|------------------|---------|
| Construção DEFIS → demonstrativos | `Application/Handlers/DemonstrativosContabeis/ConstruirDemonstrativosHandler.cs` |
| Consolidação ECD (fonte única) | `Application/Handlers/DemonstrativosContabeis/GetDClByAnoAndCodigoHandler.cs` |
| Sinal D/C | `Application/Helpers/SaldoContabilHelper.cs` (C/azul → +; Magnitude em `3.01.01*`, PMP e CJ) |
| Motor de indicadores | `Application/Handlers/Indicadores/CalcIndicadoresHandler.cs` |
| Fórmulas dos indicadores | `Domain/Resources/Indicadores.json` |
| DTO da view de balanço | `Domain/Contracts/Views/BPViewDto.cs` |
| Listagem paginada BP | `Application/Handlers/Views/BalancoPatrimonial/GetAllBPViewHandler.cs` |
| Listagem paginada DRE | `Application/Handlers/Views/DRE/GetAllDREViewHandler.cs` |
| View SQL BP | `vw_balanco_patrimonial` (`val_cta_ref_ini IS NOT NULL`) |
| View SQL DRE | `vw_dre` |

---

## Contato / dúvidas

Em caso de divergência entre DEFIS e tela, comparar primeiro o retorno de `balanco-patrimonial` para o par `(codigo, ano)`. Se a API estiver correta e a tela errada, o ajuste é no frontend conforme este documento. Se a API estiver errada após `/construir`, reportar com `id_empresa`, `ano`, `codigo` e payload da DEFIS.
