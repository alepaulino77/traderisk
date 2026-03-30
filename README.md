# Trade Risk API (.NET 8)

API REST em ASP.NET Core 8 para classificação de risco de trades, com arquitetura em camadas, regras extensíveis, validação, logging e testes.

## Visão geral

A solução foi organizada em 3 camadas principais:

- **Domain**: entidades, contratos e regras de classificação
- **Application**: casos de uso, DTOs, validação e resumo estatístico
- **Web/API**: controllers, configuração, middleware, Swagger e logging

Também há dois projetos de teste:

- **TradeRiskApi.UnitTests**
- **TradeRiskApi.IntegrationTests**

## Regras implementadas

- **LOWRISK**: trade com `value < 1_000_000`
- **MEDIUMRISK**: trade com `value >= 1_000_000` e `clientSector = Public`
- **HIGHRISK**: trade com `value >= 1_000_000` e `clientSector = Private`

## Decisões técnicas

### 1. Extensibilidade das regras
As regras foram implementadas via a interface `ITradeClassificationRule`.

Isso permite adicionar novas categorias futuramente apenas criando uma nova classe de regra e registrando-a no DI.

### 2. Processamento eficiente
O endpoint de resumo processa os trades em **uma única passagem**:

- classifica
- contabiliza quantidade por categoria
- soma o valor agregado
- calcula a maior exposição por cliente em cada categoria

A complexidade principal é **O(n)** para classificação e agregação, adequada para lotes de até 100.000 trades.

### 3. Validação
Foi usado **FluentValidation** para:

- garantir lista não vazia
- limitar o batch a 100.000 itens
- validar `value >= 0`
- validar `clientSector` como `Public` ou `Private`

### 4. Erros e logging
A API usa middleware global para:

- tratar erros de validação com HTTP 400
- tratar erros inesperados com HTTP 500
- registrar logs básicos

## Endpoints

### POST `/api/trades/risk/classify`
Classifica os trades e retorna apenas as categorias.

#### Exemplo de request

```json
[
  { "value": 2000000, "clientSector": "Private", "clientId": "CLI003" },
  { "value": 400000, "clientSector": "Public", "clientId": "CLI001" },
  { "value": 500000, "clientSector": "Public", "clientId": "CLI001" },
  { "value": 3000000, "clientSector": "Public", "clientId": "CLI002" }
]
```

#### Exemplo de response

```json
{
  "categories": ["HIGHRISK", "LOWRISK", "LOWRISK", "MEDIUMRISK"]
}
```

### POST `/api/trades/risk/classify-with-summary`
Classifica os trades e retorna resumo estatístico.

#### Exemplo de response

```json
{
  "categories": ["HIGHRISK", "LOWRISK", "LOWRISK", "MEDIUMRISK"],
  "summary": {
    "LOWRISK": {
      "count": 2,
      "totalValue": 900000,
      "topClient": "CLI001"
    },
    "MEDIUMRISK": {
      "count": 1,
      "totalValue": 3000000,
      "topClient": "CLI002"
    },
    "HIGHRISK": {
      "count": 1,
      "totalValue": 2000000,
      "topClient": "CLI003"
    }
  },
  "processingTimeMs": 45
}
```

## Como executar

### Pré-requisitos

- .NET 8 SDK

### Passos

```bash
git clone <repo>
cd trade-risk-api

dotnet restore
dotnet build
dotnet test

dotnet run --project src/TradeRiskApi.Web
```

Swagger ficará disponível em ambiente de desenvolvimento ( URL: https://localhost:44303/swagger ).

### Testando 100.000 (Cem mil) trades

Devido à limitação de tamanho do response no Swagger, para fazer uma requisição de 100.000 (Cem mil) trades, criei um script powershell test-classify.ps1 na pasta scripts. 

Ao executa-lo ele lê um arquivo JSon ( trades.json ) com um payload contendo 100.000 trades e posta na requisisão do endpoint /api/trades/risk/classify. 

Também é possível gerar outros arquivos de payload com dados aleatórios rodando o script randomic-request-generator.ps1.

```bash
./test-classify.ps1
```
OBS: Necessário ter o CURL instalado !!!

## Estrutura da solução

```text
trade-risk-api/
├── scripts/
│   ├── randomic-request-generator.ps1
│   ├── test-classify.ps1
│   └── trades.json
├── src/
│   ├── TradeRiskApi.Domain/
│   ├── TradeRiskApi.Application/
│   └── TradeRiskApi.Web/
├── tests/
│   ├── TradeRiskApi.UnitTests/
│   └── TradeRiskApi.IntegrationTests/
└── README.md
```

## Pontos que eu destacaria numa apresentação

- separação clara de responsabilidades
- facilidade de evolução das regras
- processamento linear para grandes lotes
- cobertura com testes unitários e de integração
- endpoint específico para classificação simples e outro para classificação + analytics

## Melhorias futuras

- versionamento da API
- benchmarks com BenchmarkDotNet
- observabilidade com OpenTelemetry
- autenticação/autorização
- paginação ou processamento assíncrono para lotes ainda maiores
- contratos OpenAPI enriquecidos com exemplos
