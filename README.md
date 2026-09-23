# Meu MEI — .NET MAUI

Aplicativo de utilidades para MEI, preparado para Android e Windows com uma base C#/XAML. O foco da versão 1.0 é simplicidade, operação offline e custo operacional mínimo.

## Versão 1.0.0

- Dashboard de receitas, despesas, saldo e projeção anual.
- Acompanhamento do limite anual e limite proporcional para abertura durante o ano.
- Receitas e despesas em SQLite local.
- Clientes.
- Orçamentos com compartilhamento nativo.
- Lembretes de DAS e DASN-SIMEI.
- Calculadoras de valor/hora e preço por margem.
- Dados cadastrais do MEI.
- Primeiro ano grátis por 365 dias.
- Login Google nativo no Android via Credential Manager.
- Validação do ID Token e emissão da sessão pelo DingousChatTrade.
- Interface responsiva para mobile e desktop.

## Stack

- .NET 10 + .NET MAUI
- CommunityToolkit.Mvvm 8.4.2
- sqlite-net-pcl 1.11.285
- AndroidX Credential Manager / Sign in with Google
- Shell Navigation
- SecureStorage para a sessão autenticada

## Backend

O backend do aplicativo é o **DingousChatTrade**. A versão 1.0 usa o endpoint já existente:

`POST https://dingous.com.br/api/auth/google-game`

O aplicativo obtém o ID Token pelo Google nativo no Android, envia somente esse token ao backend e armazena o JWT retornado usando `SecureStorage`. Client secret do Google não fica no aplicativo.

Os dados operacionais do MVP (lançamentos, clientes, orçamentos, perfil e obrigações) continuam offline-first em SQLite. Isso mantém o app utilizável sem internet e sem criar custo adicional por usuário.

## Google nativo — checklist de produção

O package id Android desta versão é:

`br.com.dingous.meumei`

Antes de publicar:

1. Cadastre esse package id no projeto Google usado pelo Dingous.
2. Cadastre o SHA-1/SHA-256 da chave usada para assinar a versão de produção.
3. Garanta que o OAuth Web/Server Client ID usado pelo app seja aceito como `audience` pelo endpoint `api/auth/google-game` do DingousChatTrade.
4. Faça um login real usando o APK/AAB assinado para validar a configuração de produção.

Nenhum `ClientSecret` deve ser incluído no projeto MAUI.

## Abrindo no Visual Studio

1. Instale o workload **.NET MAUI** do Visual Studio.
2. Abra `MEIUtil.sln`.
3. Restaure os pacotes NuGet.
4. Selecione Android ou Windows.
5. Execute.

Via CLI, com .NET 10 e workload MAUI instalados:

```bash
dotnet restore
dotnet build -f net10.0-android
dotnet build -f net10.0-windows10.0.19041.0
```

## Persistência

O SQLite é criado em:

`FileSystem.AppDataDirectory/meiutil.db3`

A inicialização é protegida contra chamadas concorrentes e o seed das obrigações é idempotente.

## Primeiro ano grátis

`TrialService` inicia o período de 365 dias na primeira execução e mantém o estado em `Preferences`. Nesta versão o período continua local; reinstalar/limpar dados pode reiniciá-lo. Vincular a licença ao backend mudaria a regra de produto e não faz parte deste hardening da versão 1.0.

## Obrigações

A tela de obrigações é um **lembrete**, não um substituto do Portal do Simples Nacional/PGMEI. O usuário deve confirmar guia, valor, feriados e eventual prorrogação antes do pagamento.

## Escopo deste hardening

Foram priorizados robustez, validação, mensagens de erro, estados vazios, melhor aproveitamento de espaço em desktop, áreas de toque e leitura no mobile e consistência visual. Não foram adicionados CI/CD, AppSettings, serviços pagos nem funcionalidades de negócio fora do escopo existente.
