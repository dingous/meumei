using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class ClientsPage : ContentPage
{
    private readonly ClientsViewModel _vm;
    public ClientsPage()
    {
        InitializeComponent();
        BindingContext = _vm = MauiProgram.GetRequiredService<ClientsViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
