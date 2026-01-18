using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using BankProducts.Core.Data;
using BankProducts.Core.Models;
using BankProducts.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;

namespace BankProducts.Desktop.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private BankProductsDbContext? _context;
    private IDepositService? _depositService;

    [ObservableProperty]
    private ViewModelBase? _currentView;

    [ObservableProperty]
    private string _currentViewTitle = "Банки";

    [ObservableProperty]
    private bool _isInitializing = true;

    [ObservableProperty]
    private string _initializationStatus = "Инициализация...";

    public BanksViewModel? BanksViewModel { get; private set; }
    public DepositsViewModel? DepositsViewModel { get; private set; }

    public MainWindowViewModel()
    {
        // Initialize asynchronously to avoid blocking the UI thread
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        try
        {
            InitializationStatus = "Свързване с базата данни...";
            
            // MySQL connection string - same as API
            var connectionString = "Server=localhost;Database=BankProducts;User=root;Password=;Port=3306;";

            var options = new DbContextOptionsBuilder<BankProductsDbContext>()
                .UseMySql(connectionString, 
                    new MySqlServerVersion(new Version(8, 0, 21)))
                .Options;

            _context = new BankProductsDbContext(options);
            _depositService = new DepositService(_context);

            InitializationStatus = "Инициализация на базата данни...";
            
            // Ensure database is created and seeded
            await DataSeeder.SeedAsync(_context);

            InitializationStatus = "Зареждане на данни...";

            BanksViewModel = new BanksViewModel(_depositService);
            DepositsViewModel = new DepositsViewModel(_depositService);

            // Load initial data sequentially to avoid DbContext concurrency issues
            await BanksViewModel.LoadBanks();
            
            CurrentView = BanksViewModel;
            IsInitializing = false;
        }
        catch (Exception ex)
        {
            InitializationStatus = $"Грешка: {ex.Message}";
            // Keep IsInitializing true to show the error
            // You might want to add error handling UI here
        }
    }

    [RelayCommand]
    private void ShowBanks()
    {
        if (BanksViewModel == null) return;
        CurrentView = BanksViewModel;
        CurrentViewTitle = "Банки";
        BanksViewModel.LoadBanksCommand.Execute(null);
    }

    [RelayCommand]
    private void ShowDeposits()
    {
        if (DepositsViewModel == null) return;
        CurrentView = DepositsViewModel;
        CurrentViewTitle = "Депозити";
        DepositsViewModel.LoadDepositsCommand.Execute(null);
    }
}
