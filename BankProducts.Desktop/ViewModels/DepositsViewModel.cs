using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using BankProducts.Core.Models;
using BankProducts.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BankProducts.Desktop.ViewModels;

public partial class DepositsViewModel : ViewModelBase
{
    private readonly IDepositService _depositService;

    [ObservableProperty]
    private ObservableCollection<Deposit> _deposits = new();

    [ObservableProperty]
    private ObservableCollection<Bank> _banks = new();

    [ObservableProperty]
    private Deposit? _selectedDeposit;

    [ObservableProperty]
    private bool _isEditing;

    // New deposit fields
    [ObservableProperty]
    private Bank? _newDepositBank;

    [ObservableProperty]
    private string _newDepositName = string.Empty;

    [ObservableProperty]
    private string _newDepositDescription = string.Empty;

    [ObservableProperty]
    private decimal _newDepositMinAmount;

    [ObservableProperty]
    private decimal _newDepositInterestRate;

    [ObservableProperty]
    private int _newDepositTermMonths;

    [ObservableProperty]
    private Currency _newDepositCurrency = Currency.BGN;

    [ObservableProperty]
    private bool _newDepositHasCapitalization;

    [ObservableProperty]
    private bool _newDepositAllowsAdditionalDeposits;

    [ObservableProperty]
    private bool _newDepositAllowsPartialWithdrawal;

    [ObservableProperty]
    private bool _newDepositAutoRenewal;

    // Edit deposit fields
    [ObservableProperty]
    private string _editDepositName = string.Empty;

    [ObservableProperty]
    private string _editDepositDescription = string.Empty;

    [ObservableProperty]
    private decimal _editDepositMinAmount;

    [ObservableProperty]
    private decimal _editDepositInterestRate;

    [ObservableProperty]
    private int _editDepositTermMonths;

    [ObservableProperty]
    private Currency _editDepositCurrency = Currency.BGN;

    [ObservableProperty]
    private bool _editDepositHasCapitalization;

    [ObservableProperty]
    private bool _editDepositAllowsAdditionalDeposits;

    [ObservableProperty]
    private bool _editDepositAllowsPartialWithdrawal;

    [ObservableProperty]
    private bool _editDepositAutoRenewal;

    public Currency[] CurrencyOptions { get; } = new[] { Currency.BGN, Currency.EUR, Currency.USD };

    public DepositsViewModel(IDepositService depositService)
    {
        _depositService = depositService;
        // Don't load automatically - will be loaded explicitly after initialization
    }

    [RelayCommand]
    public async Task LoadDeposits()
    {
        var deposits = await _depositService.GetAllDepositsAsync();
        Deposits = new ObservableCollection<Deposit>(deposits);

        var banks = await _depositService.GetAllBanksAsync();
        Banks = new ObservableCollection<Bank>(banks);
    }

    [RelayCommand]
    private async Task AddDeposit()
    {
        if (NewDepositBank == null || string.IsNullOrWhiteSpace(NewDepositName))
            return;

        var deposit = new Deposit
        {
            BankId = NewDepositBank.Id,
            Name = NewDepositName,
            Description = NewDepositDescription,
            MinAmount = NewDepositMinAmount,
            InterestRate = NewDepositInterestRate,
            TermMonths = NewDepositTermMonths,
            Currency = NewDepositCurrency,
            HasCapitalization = NewDepositHasCapitalization,
            AllowsAdditionalDeposits = NewDepositAllowsAdditionalDeposits,
            AllowsPartialWithdrawal = NewDepositAllowsPartialWithdrawal,
            AutoRenewal = NewDepositAutoRenewal
        };

        await _depositService.CreateDepositAsync(deposit);
        ClearNewDepositFields();
        await LoadDeposits();
    }

    private void ClearNewDepositFields()
    {
        NewDepositBank = null;
        NewDepositName = string.Empty;
        NewDepositDescription = string.Empty;
        NewDepositMinAmount = 0;
        NewDepositInterestRate = 0;
        NewDepositTermMonths = 0;
        NewDepositCurrency = Currency.BGN;
        NewDepositHasCapitalization = false;
        NewDepositAllowsAdditionalDeposits = false;
        NewDepositAllowsPartialWithdrawal = false;
        NewDepositAutoRenewal = false;
    }

    [RelayCommand]
    private void StartEdit()
    {
        if (SelectedDeposit == null) return;

        EditDepositName = SelectedDeposit.Name;
        EditDepositDescription = SelectedDeposit.Description ?? string.Empty;
        EditDepositMinAmount = SelectedDeposit.MinAmount;
        EditDepositInterestRate = SelectedDeposit.InterestRate;
        EditDepositTermMonths = SelectedDeposit.TermMonths;
        EditDepositCurrency = SelectedDeposit.Currency;
        EditDepositHasCapitalization = SelectedDeposit.HasCapitalization;
        EditDepositAllowsAdditionalDeposits = SelectedDeposit.AllowsAdditionalDeposits;
        EditDepositAllowsPartialWithdrawal = SelectedDeposit.AllowsPartialWithdrawal;
        EditDepositAutoRenewal = SelectedDeposit.AutoRenewal;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveEdit()
    {
        if (SelectedDeposit == null || string.IsNullOrWhiteSpace(EditDepositName))
            return;

        SelectedDeposit.Name = EditDepositName;
        SelectedDeposit.Description = EditDepositDescription;
        SelectedDeposit.MinAmount = EditDepositMinAmount;
        SelectedDeposit.InterestRate = EditDepositInterestRate;
        SelectedDeposit.TermMonths = EditDepositTermMonths;
        SelectedDeposit.Currency = EditDepositCurrency;
        SelectedDeposit.HasCapitalization = EditDepositHasCapitalization;
        SelectedDeposit.AllowsAdditionalDeposits = EditDepositAllowsAdditionalDeposits;
        SelectedDeposit.AllowsPartialWithdrawal = EditDepositAllowsPartialWithdrawal;
        SelectedDeposit.AutoRenewal = EditDepositAutoRenewal;

        await _depositService.UpdateDepositAsync(SelectedDeposit);
        IsEditing = false;
        await LoadDeposits();
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
    }

    [RelayCommand]
    private async Task DeleteDeposit()
    {
        if (SelectedDeposit == null) return;

        await _depositService.DeleteDepositAsync(SelectedDeposit.Id);
        SelectedDeposit = null;
        await LoadDeposits();
    }
}
