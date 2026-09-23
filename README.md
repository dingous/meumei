# Meu MEI — .NET MAUI

Aplicativo de utilidades para MEI em .NET MAUI, com Android e Windows na mesma base C#/XAML.

## Versão 1.0.7

Revisão de hardening, consistência e acabamento. Não cria módulos de negócio novos.

### Correções desta revisão

- separação fiscal/financeira consolidada: caixa usa apenas recebido/pago, enquanto o faturamento do MEI considera as vendas e serviços realizados, inclusive a prazo;
- registros futuros corrompidos/importados deixam de antecipar caixa ou faturamento antes da data;
- instalação nova deixa de criar automaticamente DAS/DASN com data-base já passada, evitando falsas pendências;
- obrigações já existentes no aparelho continuam preservadas;
- risco de faturamento diferencia aproximação e extrapolação do limite;
- estilos de botões foram unificados em uma base comum para manter altura, padding, cantos e estados consistentes;
- botões ganharam feedback leve de hover/pressed no desktop e toque;
- versão avançada para 1.0.7 / build 8.

### Mantido

- CNPJ alfanumérico e validação local CPF/CNPJ;
- Google nativo Android via Credential Manager;
- logout limpando Credential Manager;
- sessão Dingous em SecureStorage;
- SQLite local com ordenação determinística;
- primeiro ano grátis;
- proteção contra gravação duplicada e exclusão acidental;
- safe areas MAUI 10;
- Auto Backup Android desativado;
- flyout responsivo no desktop;
- nenhum CI/CD ou AppSettings alterado.

## Faturamento x caixa

O indicador de limite anual usa a receita bruta das vendas e serviços na data da operação, mesmo quando o valor ainda está a receber.

Os cards Recebido, Pago e Saldo em caixa usam somente movimentações efetivamente recebidas/pagas.

## Backend

O login Android usa:

`POST https://dingous.com.br/api/auth/google-game`

O Meu MEI usa escopo neutro `CompanyId = 0`. O DingousChatTrade emite esse token com role interna `Identity`, scope `identity` e audience `DingousIdentity`, portanto ele não é aceito pelos endpoints normais de chat/empresa.

## Persistência

SQLite local:

`FileSystem.AppDataDirectory/meiutil.db3`

## Build local

```bash
dotnet restore
dotnet build -c Release -f net10.0-android
dotnet build -c Release -f net10.0-windows10.0.19041.0
```

Antes da publicação Android, valide o AAB assinado em aparelho real, incluindo login e logout Google.
