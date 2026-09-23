# Meu MEI — .NET MAUI

Aplicativo de utilidades para MEI em .NET MAUI, com Android e Windows na mesma base C#/XAML. A versão atual prioriza confiabilidade, privacidade, responsividade e custo operacional mínimo.

## Versão 1.0.4

Esta é uma revisão de hardening e acabamento. Não cria módulos de negócio novos.

### Correções desta revisão

- o limite anual considera a receita bruta efetivamente recebida e ignora receitas anteriores à data de abertura do MEI;
- saldo mensal considera somente receitas recebidas e despesas pagas;
- login Dingous deixa de forçar o tenant 1 e usa sessão de identidade sem empresa fixa;
- o endpoint de autenticação Dingous aceita CompanyId 0 apenas como escopo neutro e continua rejeitando valores negativos;
- páginas respeitam explicitamente as safe areas do .NET MAUI 10;
- formulários críticos respeitam também teclado/soft input com SafeAreaEdges=All;
- salvamentos ficam protegidos contra duplicação caso o registro seja gravado e a navegação de retorno falhe;
- exclusão de lançamento exige confirmação;
- Auto Backup Android foi desativado para manter os dados financeiros locais fora do backup em nuvem do sistema;
- versão do app avançada para 1.0.4 / build 5.
- calendário evita afirmar atraso definitivo quando usa apenas uma data-base local; após a data, orienta revisar o prazo oficial.
- cálculos tratam overflow de valores extremos sem derrubar a tela.
- manifest Windows foi alinhado ao padrão MAUI com resources, tile e splash.

### Mantido das revisões anteriores

- suporte ao CNPJ alfanumérico vigente em 2026;
- validação local de CPF e CNPJ, inclusive dígitos verificadores do novo CNPJ;
- SQLite protegido contra inicialização concorrente;
- obrigações respeitam a data de abertura do MEI;
- sessão Dingous em SecureStorage;
- Credential Manager + Sign in with Google no Android;
- cards, listas e formulários responsivos em mobile e desktop;
- flyout desktop adaptável para janelas estreitas;
- estados vazios, loading e mensagens de erro consistentes.

Não foram adicionados CI/CD, AppSettings, serviços pagos ou infraestrutura nova.

## Stack

- .NET 10 + .NET MAUI
- CommunityToolkit.Mvvm 8.4.2
- sqlite-net-pcl 1.11.285
- Xamarin.AndroidX.Credentials 1.6.0.1
- Xamarin.AndroidX.Credentials.PlayServicesAuth 1.6.0.2
- Xamarin.Google.Android.Libraries.Identity.GoogleId 1.1.0.16
- Shell Navigation
- SecureStorage

## Backend

O backend é o **DingousChatTrade**.

O login Android usa:

`POST https://dingous.com.br/api/auth/google-game`

O app obtém o ID Token pelo Google nativo, envia esse token ao DingousChatTrade e guarda o JWT retornado no SecureStorage. Nenhum ClientSecret Google é incluído no aplicativo.

Para o Meu MEI, o pedido de autenticação usa `CompanyId = 0`, evitando vincular todo usuário ao tenant 1. O token continua com role Customer, mas sem empresa fixa.

## CNPJ alfanumérico

O cadastro aceita CNPJ numérico tradicional e CNPJ alfanumérico de 14 posições. As doze primeiras posições podem conter letras A-Z e números; as duas últimas são dígitos verificadores numéricos.

A validação é local, por módulo 11, sem API externa e sem custo por consulta.

## Persistência e privacidade

Os dados operacionais permanecem no SQLite local:

`FileSystem.AppDataDirectory/meiutil.db3`

Isso inclui lançamentos, clientes, orçamentos, perfil e lembretes.

No Android, o Auto Backup foi desativado porque o aplicativo manipula informações financeiras e o produto comunica armazenamento local.

## Google nativo — checklist de produção

Package id Android:

`br.com.dingous.meumei`

Antes da publicação:

1. mantenha esse package id cadastrado no projeto Google;
2. cadastre SHA-1/SHA-256 da chave de assinatura de produção;
3. confirme que o Server/Web Client ID usado pelo app corresponde a um audience aceito pelo DingousChatTrade;
4. valide o login usando o AAB/APK assinado.

## Build local

Com .NET 10 e workload MAUI instalados:

```bash
dotnet restore
dotnet build -c Release -f net10.0-android
dotnet build -c Release -f net10.0-windows10.0.19041.0
```

## Primeiro ano grátis

O período de 365 dias continua local em `Preferences`, conforme o escopo original. Reinstalar ou limpar os dados pode reiniciar esse período. Transferir essa licença para o backend seria uma alteração de regra de produto e não faz parte deste hardening.

## Obrigações

A tela de obrigações é um lembrete. Guia, valor, feriados e eventuais prorrogações devem ser confirmados no canal oficial antes do pagamento.
