using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class CalculatorsPage : ContentPage
{
    public CalculatorsPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.GetRequiredService<CalculatorsViewModel>();
    }
}
