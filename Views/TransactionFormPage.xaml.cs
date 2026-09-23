using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class TransactionFormPage : ContentPage, IQueryAttributable
{
    private readonly TransactionFormViewModel _vm;

    public TransactionFormPage()
    {
        InitializeComponent();
        BindingContext = _vm = MauiProgram.GetRequiredService<TransactionFormViewModel>();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query) => _vm.ApplyQuery(query);
}
