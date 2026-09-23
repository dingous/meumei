using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class QuoteFormPage : ContentPage
{
    public QuoteFormPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.GetRequiredService<QuoteFormViewModel>();
    }
}
