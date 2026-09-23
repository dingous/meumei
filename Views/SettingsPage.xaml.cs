using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class SettingsPage : ContentPage
{
    private readonly SettingsViewModel _vm;
    public SettingsPage()
    {
        InitializeComponent();
        BindingContext = _vm = MauiProgram.GetRequiredService<SettingsViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
