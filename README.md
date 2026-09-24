# Meu MEI — .NET MAUI

Aplicativo de utilidades para MEI em .NET MAUI, com Android e Windows na mesma base C#/XAML.

## Versão 1.0.9

Revisão de hardening e acabamento sem criação de novas funcionalidades.

### Correções desta revisão

- corrigido erro de compilação no logout: SettingsViewModel agora usa AuthSessionService.ClearAsync();
- SecureStorage corrompido é descartado automaticamente para não repetir falha em toda leitura;
- dashboard, financeiro, clientes e orçamentos protegem navegação contra clique/toque duplo;
- compartilhamento de orçamento entra em estado busy e não abre duas folhas simultaneamente;
- obrigações atualizam sem alternância desnecessária do estado de carregamento;
- exatamente 100% do limite passa a mostrar “No limite anual”, em vez de “Acima do limite”;
- saldo negativo ganha indicação visual de atenção;
- banner do primeiro ano muda de título quando o período termina;
- versão avançada para 1.0.9 / build 10.

### Mantido

- faturamento fiscal separado do fluxo de caixa;
- vendas e serviços a prazo entram no faturamento na data da operação;
- CNPJ alfanumérico;
- Google nativo Android via Credential Manager;
- sessão Dingous em SecureStorage;
- token neutro do Meu MEI isolado dos endpoints normais de chat;
- SQLite local;
- safe areas MAUI 10;
- Auto Backup Android desativado;
- responsividade mobile/desktop;
- nenhum CI/CD, AppSettings ou serviço pago novo.

## Backend

O login Android usa:

`POST https://dingous.com.br/api/auth/google-game`

O Meu MEI usa `CompanyId = 0`; esse token é destinado à identidade do usuário e não ao escopo normal de chat/empresa.

## Persistência

SQLite local:

`FileSystem.AppDataDirectory/meiutil.db3`

Sessão autenticada fica no SecureStorage do sistema operacional.

## Build local

```bash
dotnet restore
dotnet build -c Release -f net10.0-android
dotnet build -c Release -f net10.0-windows10.0.19041.0
```

Antes da publicação Android, valide o AAB assinado em aparelho real, incluindo login, logout e novo login Google.
