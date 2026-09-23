using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class QuotesPage : ContentPage
{
    private readonly QuotesViewModel _vm;
    public QuotesPage()
    {
        InitializeComponent();
        BindingContext = _vm = MauiProgram.GetRequiredService<QuotesViewModel>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
