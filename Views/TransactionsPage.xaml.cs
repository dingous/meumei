using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class TransactionsPage : ContentPage
{
    private readonly TransactionsViewModel _vm;

    public TransactionsPage()
    {
        InitializeComponent();
        BindingContext = _vm = MauiProgram.GetRequiredService<TransactionsViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
