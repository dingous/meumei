using MEIUtil.ViewModels;

namespace MEIUtil.Views;

public partial class ClientFormPage : ContentPage
{
    public ClientFormPage()
    {
        InitializeComponent();
        BindingContext = MauiProgram.GetRequiredService<ClientFormViewModel>();
    }
}
