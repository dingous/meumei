# Meu MEI — .NET MAUI

Aplicativo de utilidades para MEI em .NET MAUI, com Android e Windows na mesma base C#/XAML. A versão atual prioriza confiabilidade, privacidade, responsividade e custo operacional mínimo.

## Versão 1.0.5

Esta é uma revisão de hardening e acabamento. Não cria módulos de negócio novos.

### Correções desta revisão

- logout Google no Android agora limpa também o estado do Credential Manager, além do JWT do Dingous;
- novo cadastro deixa de assumir abertura em 1º de janeiro e usa a data atual como default conservador;
- cálculo mensal ignora lançamentos anteriores à data de abertura do MEI;
- estado corrompido com data de abertura futura deixa de contaminar os totais do dashboard;
- transações, clientes, orçamentos e obrigações ganharam ordenação determinística para evitar troca visual de posição em itens empatados;
- validação do CNPJ alfanumérico deixou de rejeitar indevidamente bases repetidas que tenham dígitos verificadores válidos;
- campos de texto passam a exibir botão de limpar durante a edição;
- botões desabilitados ganharam estado visual mais claro;
- versão avançada para 1.0.5 / build 6.

### Mantido das revisões anteriores

- suporte ao CNPJ alfanumérico vigente em 2026;
- validação local de CPF e CNPJ;
- receita anual baseada no que foi efetivamente recebido;
- obrigações respeitando a data de abertura;
- proteção contra salvamentos duplicados e exclusão acidental;
- safe areas do .NET MAUI 10;
- SQLite protegido contra inicialização concorrente;
- login Google nativo via Credential Manager;
- sessão Dingous em SecureStorage;
- Auto Backup Android desativado;
- cards, listas e formulários responsivos em mobile e desktop;
- flyout desktop adaptável para janelas estreitas;
- manifest Windows com resources, tiles e splash.

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

Para o Meu MEI, a autenticação usa `CompanyId = 0`, evitando vincular todos os usuários a um tenant comercial fixo.

## Persistência e privacidade

Os dados operacionais permanecem no SQLite local:

`FileSystem.AppDataDirectory/meiutil.db3`

Isso inclui lançamentos, clientes, orçamentos, perfil e lembretes.

No Android, o Auto Backup permanece desativado.

## Google nativo — checklist de produção

Package id Android:

`br.com.dingous.meumei`

Antes da publicação:

1. mantenha esse package id cadastrado no projeto Google;
2. cadastre SHA-1/SHA-256 da chave de assinatura de produção;
3. confirme que o Server/Web Client ID usado pelo app corresponde a um audience aceito pelo DingousChatTrade;
4. valide login e logout usando o AAB/APK assinado.

## Build local

Com .NET 10 e workload MAUI instalados:

```bash
dotnet restore
dotnet build -c Release -f net10.0-android
dotnet build -c Release -f net10.0-windows10.0.19041.0
```

## Primeiro ano grátis

O período de 365 dias continua local em `Preferences`, conforme o escopo original. Reinstalar ou limpar os dados pode reiniciar esse período. Transferir essa licença para o backend seria alteração de regra de produto e não faz parte deste hardening.

## Obrigações

A tela de obrigações é um lembrete. Guia, valor, feriados e eventuais prorrogações devem ser confirmados no canal oficial antes do pagamento.
