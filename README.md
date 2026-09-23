# Meu MEI — .NET MAUI

Aplicativo offline-first para organizar a rotina do Microempreendedor Individual em **Android e Windows**, com uma única base em C#/XAML.

O produto foi desenhado para ser simples para quem não quer aprender um ERP: abrir o app, registrar dinheiro que entrou/saiu, acompanhar o limite do MEI, organizar clientes, orçamentos e obrigações.

## Estado atual

- Dashboard de caixa mensal.
- Faturamento anual, limite aplicável e projeção.
- Receitas e despesas com situação recebido/pago.
- Clientes.
- Orçamentos e compartilhamento nativo.
- Calendário de apoio para DAS e DASN-SIMEI.
- Calculadoras de valor/hora e preço por margem.
- Cadastro do MEI.
- Primeiro ano grátis por 365 dias.
- SQLite local e funcionamento sem internet.
- Sincronização opcional com o backend DingousChatTrade após login.
- Login Google nativo no Android via Credential Manager.
- Login Google seguro no Windows via navegador do sistema + OAuth 2.0 Authorization Code + PKCE.
- JWT e identidade centralizados no DingousChatTrade.

## Stack

- .NET 10 + .NET MAUI
- C# / XAML
- CommunityToolkit.Mvvm 8.4.2
- sqlite-net-pcl 1.11.285
- AndroidX Credential Manager
- Google ID binding para Android
- Shell Navigation
- SecureStorage para sessão
- SQLite para dados offline

## Backend

O app usa o backend existente em `https://dingous.com.br/`.

Rotas esperadas:

- `GET /api/auth/google-native-config`
- `POST /api/auth/google-game`
- `GET /api/mei/snapshot`
- `PUT /api/mei/snapshot`

A integração correspondente está preparada no repositório `dingous/ChatTrade` no PR **#10**.

Nenhum segredo Google fica no app. O cliente recebe apenas Client IDs públicos, obtém o ID Token na plataforma e o troca por um JWT emitido pelo DingousChatTrade.

## Persistência e sincronização

O banco local fica em:

`FileSystem.AppDataDirectory/meumei.db3`

O app é **offline-first**. Sem rede ou sem login, todas as operações continuam funcionando localmente. Com uma conta Google conectada, o snapshot local é sincronizado com o DingousChatTrade.

No Android, backup automático do banco foi desativado para evitar restauração involuntária de dados financeiros em outro aparelho. Tráfego HTTP sem TLS também foi desativado.

## Primeiro ano grátis

O início do período gratuito é guardado localmente e, após login, também no backend. O app considera a data mais antiga para evitar que reinstalação ou troca de aparelho reinicie indevidamente os 365 dias.

## Google no Android

O Android usa Credential Manager com `GetSignInWithGoogleOption` e recebe um Google ID Token. Para produção, o projeto OAuth usado no Dingous precisa reconhecer o pacote:

`br.com.dingous.meumei`

Também é necessário cadastrar no Google Cloud/Play Console as impressões digitais SHA-1/SHA-256 do certificado que assinará a versão de produção. Isso é configuração da credencial Google, não exige segredo dentro do app.

## Google no Windows

Aplicativos desktop não recebem o mesmo seletor de conta do Android. O fluxo usa o navegador padrão do sistema com:

- Authorization Code;
- PKCE;
- `state` anti-CSRF;
- redirect loopback em `127.0.0.1`;
- sem Client Secret armazenado no executável.

## Rodando

Requisitos:

1. Visual Studio com workload .NET MAUI.
2. SDK .NET 10.
3. Android SDK/Emulador para Android ou Windows Machine para desktop.

Abra `MEIUtil.sln`, restaure os pacotes e execute.

CLI:

```bash
dotnet workload restore
dotnet restore
dotnet build -f net10.0-android
dotnet build -f net10.0-windows10.0.19041.0
```

## Checklist antes da publicação

- Compilar Release para Android e Windows.
- Testar instalação limpa e atualização sobre versão anterior.
- Testar banco com centenas/milhares de lançamentos.
- Testar modo avião e retorno da conectividade.
- Testar login Google com conta existente, cancelamento e ausência de conta no Android.
- Testar expiração de sessão Dingous.
- Testar sincronização em dois dispositivos antes de usar dados reais.
- Validar certificado de produção no Google Cloud/Play Console.
- Confirmar que o PR #10 do DingousChatTrade está implantado antes de publicar o app com sincronização habilitada.

## Observação sobre obrigações

A tela de obrigações é um **organizador e lembrete**. Ela não substitui PGMEI, Receita Federal ou orientação contábil e não calcula multa/juros. Datas e valores de pagamento devem ser confirmados no canal oficial.
