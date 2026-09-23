using MEIUtil.Views;

namespace MEIUtil;

public sealed class AppShell : Shell
{
    public AppShell()
    {
        FlyoutBehavior = DeviceInfo.Idiom == DeviceIdiom.Desktop
            ? FlyoutBehavior.Locked
            : FlyoutBehavior.Flyout;
        Title = "Meu MEI";
        FlyoutWidth = DeviceInfo.Idiom == DeviceIdiom.Desktop ? 276 : 300;
        FlyoutHeader = BuildHeader();
        FlyoutFooter = BuildFooter();

        AddMenu("Início", "⌂", typeof(DashboardPage), "dashboard");
        AddMenu("Receitas e despesas", "R$", typeof(TransactionsPage), "transactions");
        AddMenu("Clientes", "◉", typeof(ClientsPage), "clients");
        AddMenu("Orçamentos", "▤", typeof(QuotesPage), "quotes");
        AddMenu("Obrigações", "✓", typeof(ObligationsPage), "obligations");
        AddMenu("Calculadoras", "=", typeof(CalculatorsPage), "calculators");
        AddMenu("Meu MEI", "⚙", typeof(SettingsPage), "settings");

        Routing.RegisterRoute(nameof(TransactionFormPage), typeof(TransactionFormPage));
        Routing.RegisterRoute(nameof(ClientFormPage), typeof(ClientFormPage));
        Routing.RegisterRoute(nameof(QuoteFormPage), typeof(QuoteFormPage));
    }

    private static View BuildHeader()
    {
        var initials = new Label
        {
            Text = "M",
            TextColor = Colors.White,
            FontSize = 21,
            FontAttributes = FontAttributes.Bold,
            HorizontalTextAlignment = TextAlignment.Center,
            VerticalTextAlignment = TextAlignment.Center
        };

        var mark = new Border
        {
            WidthRequest = 48,
            HeightRequest = 48,
            BackgroundColor = Color.FromArgb("#4F46E5"),
            StrokeThickness = 0,
            StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 },
            Content = initials
        };

        var text = new VerticalStackLayout
        {
            Spacing = 1,
            VerticalOptions = LayoutOptions.Center,
            Children =
            {
                new Label { Text = "Meu MEI", FontSize = 20, FontAttributes = FontAttributes.Bold, TextColor = Color.FromArgb("#111827") },
                new Label { Text = "negócio simples, gestão clara", FontSize = 11, TextColor = Color.FromArgb("#6B7280") }
            }
        };

        return new Grid
        {
            Padding = new Thickness(18, 26, 18, 18),
            ColumnDefinitions = { new ColumnDefinition(GridLength.Auto), new ColumnDefinition(GridLength.Star) },
            ColumnSpacing = 12,
            Children = { mark, text }
        }.Tap(grid => Grid.SetColumn(text, 1));
    }

    private static View BuildFooter()
        => new VerticalStackLayout
        {
            Padding = new Thickness(18, 12, 18, 22),
            Spacing = 3,
            Children =
            {
                new BoxView { HeightRequest = 1, Color = Color.FromArgb("#E5E7EB"), Margin = new Thickness(0, 0, 0, 10) },
                new Label { Text = "Dingous ecosystem", FontSize = 11, TextColor = Color.FromArgb("#6B7280") },
                new Label { Text = $"Meu MEI {AppInfo.Current.VersionString}", FontSize = 11, TextColor = Color.FromArgb("#9CA3AF") }
            }
        };

    private void AddMenu(string title, string icon, Type pageType, string route)
    {
        Items.Add(new FlyoutItem
        {
            Title = $"{icon}   {title}",
            Route = route,
            Items =
            {
                new ShellContent
                {
                    Title = title,
                    Route = $"{route}-content",
                    ContentTemplate = new DataTemplate(pageType)
                }
            }
        });
    }
}

internal static class ViewBuilderExtensions
{
    public static T Tap<T>(this T value, Action<T> configure)
    {
        configure(value);
        return value;
    }
}
