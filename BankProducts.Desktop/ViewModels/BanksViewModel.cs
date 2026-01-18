using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BankProducts.Core.Models;
using BankProducts.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BankProducts.Desktop.ViewModels;

public partial class BanksViewModel : ViewModelBase
{
    private readonly IDepositService _depositService;

    [ObservableProperty]
    private ObservableCollection<Bank> _banks = new();

    [ObservableProperty]
    private Bank? _selectedBank;

    [ObservableProperty]
    private string _newBankName = string.Empty;

    [ObservableProperty]
    private bool _isEditing;

    [ObservableProperty]
    private string _editBankName = string.Empty;

    public BanksViewModel(IDepositService depositService)
    {
        _depositService = depositService;
        // Don't load automatically - will be loaded explicitly after initialization
    }

    [RelayCommand]
    public async Task LoadBanks()
    {
        var banks = await _depositService.GetAllBanksAsync();
        Banks = new ObservableCollection<Bank>(banks);
    }

    [RelayCommand]
    private async Task AddBank()
    {
        if (string.IsNullOrWhiteSpace(NewBankName))
            return;

        var bank = new Bank
        {
            Name = NewBankName
        };

        await _depositService.CreateBankAsync(bank);
        NewBankName = string.Empty;
        await LoadBanks();
    }

    [RelayCommand]
    private void StartEdit()
    {
        if (SelectedBank == null) return;

        EditBankName = SelectedBank.Name;
        IsEditing = true;
    }

    [RelayCommand]
    private async Task SaveEdit()
    {
        if (SelectedBank == null || string.IsNullOrWhiteSpace(EditBankName))
            return;

        SelectedBank.Name = EditBankName;

        await _depositService.UpdateBankAsync(SelectedBank);
        IsEditing = false;
        await LoadBanks();
    }

    [RelayCommand]
    private void CancelEdit()
    {
        IsEditing = false;
        EditBankName = string.Empty;
    }

    [RelayCommand]
    private async Task DeleteBank()
    {
        if (SelectedBank == null) return;

        await _depositService.DeleteBankAsync(SelectedBank.Id);
        SelectedBank = null;
        await LoadBanks();
    }
}
