using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace MEIUtil.ViewModels;

public partial class CalculatorsViewModel : ObservableObject
{
    [ObservableProperty] private decimal desiredMonthlyIncome;
    [ObservableProperty] private decimal monthlyBusinessCosts;
    [ObservableProperty] private decimal billableHours = 120m;
    [ObservableProperty] private decimal hourlyRate;
    [ObservableProperty] private decimal productCost;
    [ObservableProperty] private decimal desiredMarginPercent = 30m;
    [ObservableProperty] private decimal suggestedSalePrice;
    [ObservableProperty] private string errorMessage = string.Empty;

    [RelayCommand]
    private void CalculateHourlyRate()
    {
        ErrorMessage = string.Empty;
        if (BillableHours <= 0)
        {
            HourlyRate = 0;
            ErrorMessage = "Informe uma quantidade de horas maior que zero.";
            return;
        }
        if (DesiredMonthlyIncome < 0 || MonthlyBusinessCosts < 0)
        {
            HourlyRate = 0;
            ErrorMessage = "Use valores maiores ou iguais a zero.";
            return;
        }
        HourlyRate = Math.Round((DesiredMonthlyIncome + MonthlyBusinessCosts) / BillableHours, 2);
    }

    [RelayCommand]
    private void CalculateSalePrice()
    {
        ErrorMessage = string.Empty;
        if (ProductCost < 0 || DesiredMarginPercent < 0 || DesiredMarginPercent >= 100)
        {
            SuggestedSalePrice = 0;
            ErrorMessage = "Use custo positivo e margem entre 0% e 99,99%.";
            return;
        }
        var margin = DesiredMarginPercent / 100m;
        SuggestedSalePrice = Math.Round(ProductCost / (1m - margin), 2);
    }
}
