# Meu MEI — .NET MAUI

Aplicativo de utilidades para MEI, preparado para Android e Windows com uma base C#/XAML. O foco é simplicidade, confiabilidade, operação local e custo operacional mínimo.

## Versão 1.0.2

Esta revisão é de hardening e acabamento. Não cria módulos novos.

### Correções e melhorias

- suporte ao CNPJ alfanumérico vigente desde julho de 2026;
- validação local de CPF e CNPJ, incluindo dígitos verificadores do novo CNPJ;
- campos de CNPJ deixam de forçar teclado exclusivamente numérico;
- fluxo de caixa mensal considera somente receitas recebidas e despesas pagas;
- faturamento anual continua considerando as receitas registradas para acompanhamento do limite;
- lembretes anteriores à abertura do MEI deixam de aparecer como falsas pendências;
- seed de obrigações respeita o mês de abertura;
- proteção contra cliques concorrentes em exclusão e atualização de obrigações;
- navegação lateral do desktop recolhe automaticamente em janelas estreitas;
- período gratuito ficou mais resistente a valor local corrompido ou relógio inconsistente;
- binding Android do Credential Manager alinhado às revisões atuais;
- formulários receberam limites de tamanho, melhores teclados e hierarquia visual;
- listas, estados vazios, indicadores de carregamento e quebra responsiva foram refinados.

Não foram adicionados CI/CD, AppSettings, serviços pagos ou infraestrutura nova.

## Stack

- .NET 10 + .NET MAUI
- CommunityToolkit.Mvvm 8.4.2
- sqlite-net-pcl 1.11.285
- AndroidX Credential Manager / Sign in with Google
- Shell Navigation
- SecureStorage para a sessão autenticada

## Backend

O backend é o **DingousChatTrade**.

O login Android usa:

`POST https://dingous.com.br/api/auth/google-game`

O app obtém o ID Token pelo Google nativo, envia o token ao DingousChatTrade e armazena o JWT devolvido usando `SecureStorage`. Nenhum ClientSecret Google é incluído no aplicativo.

O endpoint existente do DingousChatTrade aceita os client IDs Google configurados no servidor como audiences válidos para o login nativo, sem exigir alteração de AppSettings.

## CNPJ alfanumérico

O cadastro aceita tanto o formato numérico tradicional quanto o formato alfanumérico de 14 posições. As 12 primeiras posições podem conter letras de A a Z e números; as duas últimas continuam sendo dígitos verificadores numéricos.

A validação é feita localmente pelo módulo 11, sem API externa e sem custo por consulta.

## Persistência

Os dados operacionais permanecem em SQLite local:

`FileSystem.AppDataDirectory/meiutil.db3`

Isso inclui lançamentos, clientes, orçamentos, perfil e lembretes.

## Google nativo — checklist de produção

Package id Android:

`br.com.dingous.meumei`

Antes da publicação:

1. mantenha esse package id cadastrado no projeto Google;
2. cadastre SHA-1/SHA-256 da chave de assinatura de produção;
3. confirme que o Server/Web Client ID usado pelo app corresponde a um audience aceito pelo DingousChatTrade;
4. valide o login no AAB/APK assinado.

## Build local

Com .NET 10 e workload MAUI instalados:

```bash
dotnet restore
dotnet build -c Release -f net10.0-android
dotnet build -c Release -f net10.0-windows10.0.19041.0
```

## Primeiro ano grátis

O período de 365 dias continua local em `Preferences`, como no escopo original. Reinstalar ou limpar os dados do aplicativo pode reiniciar esse período; mover a licença para o backend seria uma mudança de funcionalidade e não faz parte deste hardening.

## Obrigações

A tela de obrigações é um lembrete. Guia, valor, feriados e eventuais prorrogações devem ser confirmados no canal oficial antes do pagamento.
