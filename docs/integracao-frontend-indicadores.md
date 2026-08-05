# Integração frontend — Indicadores

Documento para o projeto de **frontend** (repositório separado) que consome esta API. Descreve como montar a grade de indicadores **sem alterar** as regras de cálculo já implementadas no backend.

Complementa `integracao-frontend-balanco.md`.

---

## Resumo

| Regra | Consequência |
|-------|--------------|
| As colunas de ano vêm **apenas** dos `ano` presentes em `valoresAnuais` | Nunca gerar anos por conta própria |
| `ano` chega como **string** (`"2022"`) | Converter com `Number()` antes de ordenar ou comparar |
| A ordem de `valoresAnuais` **não é garantida** | Ordenar no cliente por ano |
| `saude_empresa` já é a média calculada pelo backend | Não recalcular na tela |
| A resposta é **paginada com `page_size` padrão 10** e existem **17 indicadores** | Enviar `PageSize=50` ou percorrer as páginas |

**Não** derivar as colunas a partir do maior ano (`[maiorAno, maiorAno+1, maiorAno+2]`).

**Não** comparar a régua de anos desta tela com a do Balanço Patrimonial — ver "Por que os anos diferem do Balanço".

---

## Endpoint

```
GET /api/Indicadores/calculos_indicaroes/{idEmpresa}?PageSize=50
Headers: Authorization, X-Tenant-Id
```

> O caminho tem essa grafia (`calculos_indicaroes`) na API. Não é erro de digitação deste documento.

### Query string

| Parâmetro | Tipo | Observação |
|-----------|------|------------|
| `Page` | int | Padrão `1` |
| `PageSize` | int | Padrão `10`. **Use 50** — são 17 indicadores e aqui o backend não eleva o padrão automaticamente |
| `Sort` | string | `nome` ou `saude_empresa`; qualquer outro valor ordena por `id_indicador` |
| `OrderByDescending` | bool | Padrão `false` |
| `IdIndicador` | long | Filtra um indicador específico |
| `Ano` | int | Retorna somente indicadores que tenham aquele ano, já com `valoresAnuais` reduzido a ele |

---

## Formato da resposta

```jsonc
{
  "paging": { "page": 1, "page_size": 50, "total": 17 },
  "data": [
    {
      "id_indicador": 2908,
      "nome": "Capital de Giro de Longo Prazo (CGLP)",
      "saude_empresa": 1.1382,
      "valores_calc_saude_empresa": " (Valor2020: 0 + Valor2021: 1.7093 + Valor2022: 1.7052) / 3",
      "valoresAnuais": [
        { "id_valor": 1, "ano": "2020", "valor": 0.0000,  "formula": "…", "formula_contas": "{1.01.01} / {2.01}", "valores_calc_ano": "…" },
        { "id_valor": 2, "ano": "2021", "valor": 1.7093, "formula": "…", "formula_contas": "{1.01.01} / {2.01}", "valores_calc_ano": "…" },
        { "id_valor": 3, "ano": "2022", "valor": 1.7052, "formula": "…", "formula_contas": "{1.01.01} / {2.01}", "valores_calc_ano": "…" }
      ]
    }
  ]
}
```

Atenção às duas convenções de nome coexistindo: os campos com `[JsonPropertyName]` saem em snake_case (`id_indicador`, `saude_empresa`, `valores_calc_ano`), mas a coleção **`valoresAnuais` sai em camelCase**, porque não tem atributo próprio e cai no padrão do ASP.NET Core.

| Campo | Tipo JSON | Significado |
|-------|-----------|-------------|
| `nome` | string | Nome do indicador, igual ao `nome` em `Domain/Resources/Indicadores.json` |
| `saude_empresa` | number | Média aritmética dos `valor` de todos os anos retornados |
| `valores_calc_saude_empresa` | string | Memória de cálculo da média, para tooltip/auditoria |
| `valoresAnuais[].ano` | **string** | Exercício. Serializado como texto porque o DTO expõe `string` |
| `valoresAnuais[].valor` | number | Valor do indicador no exercício, arredondado em 6 casas |
| `valoresAnuais[].formula` | string | Descrição textual da fórmula (`Desc_Formula` do JSON de fórmulas) |
| `valoresAnuais[].formula_contas` | string | Fórmula com códigos de conta (`Formula` do JSON, ex.: `{1.01.01} / {2.01}`) |
| `valoresAnuais[].valores_calc_ano` | string | Expressão já com os números substituídos (sempre magnitudes positivas; indicador D/C não entra) |

---

## Como o backend escolhe os anos

`CalcIndicadoresHandler` monta uma **janela de 3 exercícios contíguos terminando no maior ano com demonstrativo cadastrado** (`DemonstrativosAnosHelper.ObterJanelaUltimosAnos`). Se o maior ano é 2022, a janela é 2020, 2021 e 2022.

Consequências que a tela precisa respeitar:

- A janela pode incluir um ano **sem dados**. Esse ano vem presente na resposta, com `valor: 0.0000` — ele não é omitido.
- A quantidade de anos é sempre 3 quando existe pelo menos um demonstrativo, mas trate a lista como variável: empresas sem demonstrativo nenhum retornam `valoresAnuais` vazio.
- Os anos são recalculados a cada `POST /api/demonstrativos/construir`. A tela não deve cachear a régua de anos entre empresas.

---

## Montagem da grade

### Régua de anos

Derive as colunas da união dos anos retornados, convertendo para número:

```ts
const anos = [
  ...new Set(
    data.flatMap((ind) => ind.valoresAnuais.map((v) => Number(v.ano)))
  ),
].sort((a, b) => a - b);
```

### Células

```ts
function valorDoAno(indicador: CalcIndicadorDto, ano: number): number | null {
  const item = indicador.valoresAnuais.find((v) => Number(v.ano) === ano);
  return item ? item.valor : null;
}
```

Use `null` (renderizado como `-`) apenas quando o ano **não existir** na resposta. Um ano que veio com `valor: 0` é um zero calculado e deve ser exibido como `0,0000`.

### Erro que gerou o bug relatado

```ts
// ERRADO — inventa anos que a API nunca devolveu
const maior = Math.max(...indicador.valoresAnuais.map((v) => Number(v.ano)));
const anos = [maior, maior + 1, maior + 2]; // 2022, 2023, 2024
```

Sintoma na tela: a API devolve 2020, 2021 e 2022, mas a grade mostra 2022, 2023 e 2024. A primeira coluna exibe o valor correto do último exercício, as outras duas ficam vazias e os dois anos anteriores desaparecem — inclusive o único ano comparativo com dados reais.

Caso concreto (empresa `IMAGEM PLAST AMBIENTAL RECICLADORA LTDA`, `id_empresa = 231`):

| Indicador | 2020 (API) | 2021 (API) | 2022 (API) | `saude_empresa` |
|-----------|-----------|-----------|-----------|-----------------|
| Capital de Giro de Longo Prazo (CGLP) | 0,0000 | 1,7093 | 1,7052 | 1,1382 |
| Liquidez Geral | 0,0000 | -0,1263 | -0,1270 | -0,0844 |
| Liquidez Seca | 0,0000 | -0,1263 | -0,1270 | -0,0844 |
| Composição do Endividamento | 0,0000 | 0,9698 | 0,9698 | 0,6465 |

A tela exibia 1,7052 sob o rótulo "2022" e nada em "2023"/"2024" — os números estavam certos, os rótulos não.

### Ordenação

`valoresAnuais` vem de um `Include` sem `OrderBy`; a ordem depende do plano de execução do banco. Ordene sempre no cliente:

```ts
const valores = [...indicador.valoresAnuais].sort((a, b) => Number(a.ano) - Number(b.ano));
```

O mesmo vale para comparações: `"2022" > "2021"` funciona por acaso em strings de 4 dígitos, mas `Math.max` sobre strings e chaves de `Map` mistas (`2022` vs `"2022"`) já produziram divergência. Normalize para `number` na borda da API.

---

## Coluna "Saúde da Empresa"

`saude_empresa` é a média simples dos anos da janela, **incluindo anos sem dados que valem 0**. No exemplo acima, o CGLP real oscila em torno de 1,70, mas a saúde exibida é 1,1382 porque 2020 entra como zero.

Isso é comportamento atual e deliberado do backend: não recalcule a média no frontend nem filtre zeros na hora de exibir, ou a tela deixa de bater com `valores_calc_saude_empresa` (que é a memória de cálculo mostrada ao usuário). Se a régua precisar mudar, o ajuste é no backend.

---

## Por que os anos diferem do Balanço Patrimonial

Na aba de Balanço, a coluna do ano mais antigo pode ser o **saldo de abertura** (`val_cta_ref_ini`) do exercício seguinte, exibido como se fosse um ano próprio. Uma empresa com demonstrativos apenas em 2021 e 2022 aparece no Balanço com três colunas (2020, 2021, 2022), sendo a de 2020 a abertura de 2021.

Os indicadores não fazem isso: eles usam a janela de exercícios descrita acima. Então **é esperado** que as duas abas mostrem conjuntos de anos parecidos por coincidência, mas com origens diferentes. Não use os anos de uma aba para montar a outra.

---

## Checklist de correção no frontend

- [ ] Colunas derivadas de `valoresAnuais[].ano`, nunca de um range calculado
- [ ] `Number(v.ano)` na borda da API — nada de comparar ou ordenar string de ano
- [ ] `PageSize=50` (ou paginar até esgotar `paging.total`)
- [ ] `valoresAnuais` ordenado no cliente
- [ ] Ano ausente → `-`; ano presente com `valor: 0` → `0,0000`
- [ ] `saude_empresa` exibida como veio, sem recálculo
- [ ] Tooltip de auditoria usando `valores_calc_ano` e `valores_calc_saude_empresa`
- [ ] Régua de anos recarregada após `POST /api/demonstrativos/construir`

---

## Referências no código (CapagWebAPI)

| Responsabilidade | Arquivo |
|------------------|---------|
| Endpoint da grade | `WebAPI/Controllers/IndicadoresController.cs` |
| Leitura paginada | `Application/Handlers/Indicadores/GetAllCalcIndicaoresHandler.cs` |
| Motor de cálculo | `Application/Handlers/Indicadores/CalcIndicadoresHandler.cs` |
| Janela de anos | `Application/Helpers/DemonstrativosAnosHelper.cs` |
| Anos disponíveis da empresa | `Application/Handlers/DemonstrativosContabeis/GetAnosCalculoDemonstrativoHandler.cs` |
| DTOs | `Domain/Contracts/Indicadores/CalcIndicadoresDto.cs`, `Domain/Contracts/ValoresAnuais/CalcValoresAnuaisDto.cs` |
| Fórmulas e descrições | `Domain/Resources/Indicadores.json` |
