using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using MEIUtil.Services;
using MEIUtil.Services.Auth;
using MEIUtil.ViewModels;
#if ANDROID
using MEIUtil.Platforms.Android.Services;
#endif

namespace MEIUtil;

public static class MauiProgram
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static MauiApp CreateMauiApp()
    {
        var culture = CultureInfo.GetCultureInfo("pt-BR");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        var builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();

        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<MeiRulesService>();
        builder.Services.AddSingleton<TrialService>();
        builder.Services.AddSingleton<SeedService>();
        builder.Services.AddSingleton<AuthSessionService>();
        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://dingous.com.br/"),
            Timeout = TimeSpan.FromSeconds(20)
        });
        builder.Services.AddSingleton<DingousAuthApi>();

#if ANDROID
        builder.Services.AddSingleton<IGoogleNativeAuthService, AndroidGoogleNativeAuthService>();
#else
        builder.Services.AddSingleton<IGoogleNativeAuthService, UnsupportedGoogleNativeAuthService>();
#endif

        builder.Services.AddTransient<DashboardViewModel>();
        builder.Services.AddTransient<TransactionsViewModel>();
        builder.Services.AddTransient<TransactionFormViewModel>();
        builder.Services.AddTransient<ClientsViewModel>();
        builder.Services.AddTransient<ClientFormViewModel>();
        builder.Services.AddTransient<QuotesViewModel>();
        builder.Services.AddTransient<QuoteFormViewModel>();
        builder.Services.AddTransient<ObligationsViewModel>();
        builder.Services.AddTransient<CalculatorsViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();

        var app = builder.Build();
        Services = app.Services;
        return app;
    }

    public static T GetRequiredService<T>() where T : notnull
        => Services.GetRequiredService<T>();
}
