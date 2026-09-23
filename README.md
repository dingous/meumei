# Meu MEI — .NET MAUI

Aplicativo de utilidades para MEI em .NET MAUI, com Android e Windows na mesma base C#/XAML.

## Versão 1.0.6

Esta revisão mantém o mesmo escopo funcional e foca em correção, robustez, responsividade e acabamento.

### Correções desta revisão

- faturamento anual deixa de depender do status Recebido: vendas e serviços a prazo entram no limite na data da operação;
- caixa mensal continua separado e considera apenas entradas recebidas e saídas pagas;
- mensagem de risco agora diferencia quando o limite já foi ultrapassado;
- nova instalação também cria o lembrete da DASN referente ao ano anterior quando aplicável;
- datas de obrigações são apresentadas explicitamente como data-base, evitando afirmar vencimento ajustado por feriado/prorrogação;
- clientes ganharam iniciais e deixam de reservar linhas vazias para telefone/e-mail ausentes;
- ações de Clientes e Orçamentos quebram melhor em telas estreitas;
- versão avançada para 1.0.6 / build 7.

### Mantido

- CNPJ alfanumérico;
- validação local de CPF/CNPJ;
- Google nativo Android via Credential Manager;
- logout limpando Credential Manager;
- sessão Dingous em SecureStorage;
- SQLite local;
- primeiro ano grátis;
- proteção contra gravação duplicada e exclusão acidental;
- safe areas MAUI 10;
- Auto Backup Android desativado;
- flyout responsivo no desktop;
- nenhum CI/CD ou AppSettings alterado.

## Regra de faturamento

O indicador do limite anual usa a receita bruta das vendas e serviços registrados no período. O status Recebido/Pago é usado somente para o caixa financeiro. Assim, uma venda a prazo conta no faturamento na data da venda, mesmo que ainda esteja a receber.

## Backend

O login Android usa:

`POST https://dingous.com.br/api/auth/google-game`

A autenticação do Meu MEI usa escopo neutro `CompanyId = 0`, sem vincular o usuário a um tenant comercial fixo.

## Persistência

SQLite local:

`FileSystem.AppDataDirectory/meiutil.db3`

## Build local

```bash
dotnet restore
dotnet build -c Release -f net10.0-android
dotnet build -c Release -f net10.0-windows10.0.19041.0
```

## Produção

Antes de publicar Android, valide o AAB assinado em aparelho real, inclusive login e logout Google com SHA-1/SHA-256 de produção.
