using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class ObligationsPage : ContentPage
{
    private readonly ObligationsViewModel _vm;
    public ObligationsPage()
    {
        InitializeComponent();
        BindingContext = _vm = MauiProgram.GetRequiredService<ObligationsViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
