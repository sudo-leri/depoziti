using BankProducts.Core.Data;
using BankProducts.Core.Models;
using BankProducts.Core.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BankProducts.Tests.Services;

/// <summary>
/// Unit tests for the DepositService class.
/// Tests CRUD operations, filtering, and sorting functionality.
/// </summary>
public class DepositServiceTests : IDisposable
{
    private readonly BankProductsDbContext _context;
    private readonly DepositService _service;

    public DepositServiceTests()
    {
        var options = new DbContextOptionsBuilder<BankProductsDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new BankProductsDbContext(options);
        _service = new DepositService(_context);

        SeedTestData();
    }

    private void SeedTestData()
    {
        var banks = new List<Bank>
        {
            new Bank { Id = 1, Name = "УниКредит Булбанк" },
            new Bank { Id = 2, Name = "Банка ДСК" },
            new Bank { Id = 3, Name = "Пощенска банка" }
        };

        var deposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                BankId = 1,
                Name = "Премиум депозит",
                Description = "Депозит с висока лихва",
                MinAmount = 10000,
                MaxAmount = null,
                InterestRate = 3.5m,
                TermMonths = 12,
                Currency = Currency.BGN,
                HasCapitalization = true,
                IsActive = true
            },
            new Deposit
            {
                Id = 2,
                BankId = 1,
                Name = "Евро депозит",
                Description = "Депозит в евро",
                MinAmount = 5000,
                MaxAmount = 50000,
                InterestRate = 2.8m,
                TermMonths = 6,
                Currency = Currency.EUR,
                HasCapitalization = false,
                IsActive = true
            },
            new Deposit
            {
                Id = 3,
                BankId = 2,
                Name = "Дългосрочен депозит",
                Description = "Депозит за 24 месеца",
                MinAmount = 1000,
                MaxAmount = null,
                InterestRate = 4.2m,
                TermMonths = 24,
                Currency = Currency.BGN,
                HasCapitalization = true,
                IsActive = true
            },
            new Deposit
            {
                Id = 4,
                BankId = 3,
                Name = "Неактивен депозит",
                Description = "Вече не се предлага",
                MinAmount = 500,
                MaxAmount = null,
                InterestRate = 5.0m,
                TermMonths = 12,
                Currency = Currency.BGN,
                HasCapitalization = false,
                IsActive = false
            }
        };

        _context.Banks.AddRange(banks);
        _context.Deposits.AddRange(deposits);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllDepositsAsync_ShouldReturnOnlyActiveDeposits()
    {
        // Act
        var result = await _service.GetAllDepositsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3); // Само активните депозити
        result.Should().OnlyContain(d => d.IsActive);
    }

    [Fact]
    public async Task GetAllDepositsAsync_ShouldReturnDepositsOrderedByInterestRateDescending()
    {
        // Act
        var result = await _service.GetAllDepositsAsync();
        var depositsList = result.ToList();

        // Assert
        depositsList.Should().HaveCount(3);
        depositsList[0].InterestRate.Should().Be(4.2m);
        depositsList[1].InterestRate.Should().Be(3.5m);
        depositsList[2].InterestRate.Should().Be(2.8m);
    }

    [Fact]
    public async Task GetAllDepositsAsync_ShouldIncludeBankInformation()
    {
        // Act
        var result = await _service.GetAllDepositsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().OnlyContain(d => d.Bank != null);
        result.First().Bank.Name.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_WithBankIdFilter_ShouldReturnOnlyDepositsFromThatBank()
    {
        // Arrange
        var filter = new DepositFilter { BankId = 1 };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(d => d.BankId == 1);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_WithCurrencyFilter_ShouldReturnOnlyDepositsInThatCurrency()
    {
        // Arrange
        var filter = new DepositFilter { Currency = Currency.EUR };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);

        // Assert
        result.Should().HaveCount(1);
        result.First().Currency.Should().Be(Currency.EUR);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_WithMinTermFilter_ShouldReturnDepositsWithTermGreaterOrEqual()
    {
        // Arrange
        var filter = new DepositFilter { MinTermMonths = 12 };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(d => d.TermMonths >= 12);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_WithMaxTermFilter_ShouldReturnDepositsWithTermLessOrEqual()
    {
        // Arrange
        var filter = new DepositFilter { MaxTermMonths = 12 };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(d => d.TermMonths <= 12);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_WithMinAmountFilter_ShouldReturnDepositsAffordableWithThatAmount()
    {
        // Arrange
        var filter = new DepositFilter { MinAmount = 5000 };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);

        // Assert
        result.Should().HaveCount(2);
        result.Should().OnlyContain(d => d.MinAmount <= 5000);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_WithMultipleFilters_ShouldApplyAllFilters()
    {
        // Arrange
        var filter = new DepositFilter
        {
            Currency = Currency.BGN,
            MinTermMonths = 12,
            MinAmount = 5000
        };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);

        // Assert
        result.Should().HaveCount(1);
        var deposit = result.First();
        deposit.Currency.Should().Be(Currency.BGN);
        deposit.TermMonths.Should().BeGreaterThanOrEqualTo(12);
        deposit.MinAmount.Should().BeLessThanOrEqualTo(5000);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_SortByTerm_ShouldReturnDepositsSortedByTerm()
    {
        // Arrange
        var filter = new DepositFilter { SortBy = "term" };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);
        var depositsList = result.ToList();

        // Assert
        depositsList[0].TermMonths.Should().Be(6);
        depositsList[1].TermMonths.Should().Be(12);
        depositsList[2].TermMonths.Should().Be(24);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_SortByTermDescending_ShouldReturnDepositsSortedByTermDesc()
    {
        // Arrange
        var filter = new DepositFilter { SortBy = "term", SortDescending = true };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);
        var depositsList = result.ToList();

        // Assert
        depositsList[0].TermMonths.Should().Be(24);
        depositsList[1].TermMonths.Should().Be(12);
        depositsList[2].TermMonths.Should().Be(6);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_SortByMinAmount_ShouldReturnDepositsSortedByMinAmount()
    {
        // Arrange
        var filter = new DepositFilter { SortBy = "minamount" };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);
        var depositsList = result.ToList();

        // Assert
        depositsList[0].MinAmount.Should().Be(1000);
        depositsList[1].MinAmount.Should().Be(5000);
        depositsList[2].MinAmount.Should().Be(10000);
    }

    [Fact]
    public async Task GetFilteredDepositsAsync_SortByBank_ShouldReturnDepositsSortedByBankName()
    {
        // Arrange
        var filter = new DepositFilter { SortBy = "bank" };

        // Act
        var result = await _service.GetFilteredDepositsAsync(filter);
        var depositsList = result.ToList();

        // Assert
        depositsList[0].Bank.Name.Should().Be("Банка ДСК");
        depositsList[1].Bank.Name.Should().Be("УниКредит Булбанк");
        depositsList[2].Bank.Name.Should().Be("УниКредит Булбанк");
    }

    [Fact]
    public async Task GetDepositByIdAsync_WithValidId_ShouldReturnDeposit()
    {
        // Act
        var result = await _service.GetDepositByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("Премиум депозит");
        result.Bank.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDepositByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetDepositByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateDepositAsync_ShouldAddNewDeposit()
    {
        // Arrange
        var newDeposit = new Deposit
        {
            BankId = 1,
            Name = "Нов депозит",
            MinAmount = 2000,
            InterestRate = 3.0m,
            TermMonths = 18,
            Currency = Currency.BGN,
            IsActive = true
        };

        // Act
        var result = await _service.CreateDepositAsync(newDeposit);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var allDeposits = await _service.GetAllDepositsAsync();
        allDeposits.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateDepositAsync_ShouldUpdateExistingDeposit()
    {
        // Arrange
        var deposit = await _service.GetDepositByIdAsync(1);
        deposit!.InterestRate = 4.0m;
        deposit.Name = "Обновен депозит";

        // Act
        var result = await _service.UpdateDepositAsync(deposit);

        // Assert
        result.Should().NotBeNull();
        result.InterestRate.Should().Be(4.0m);
        result.Name.Should().Be("Обновен депозит");
        result.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task DeleteDepositAsync_WithExistingId_ShouldRemoveDeposit()
    {
        // Arrange
        var initialCount = (await _service.GetAllDepositsAsync()).Count();

        // Act
        await _service.DeleteDepositAsync(1);

        // Assert
        var deposit = await _service.GetDepositByIdAsync(1);
        deposit.Should().BeNull();

        var remainingDeposits = await _service.GetAllDepositsAsync();
        remainingDeposits.Should().HaveCount(initialCount - 1);
    }

    [Fact]
    public async Task DeleteDepositAsync_WithNonExistingId_ShouldNotThrowException()
    {
        // Act & Assert
        await FluentActions.Invoking(() => _service.DeleteDepositAsync(999))
            .Should().NotThrowAsync();
    }

    [Fact]
    public async Task GetAllBanksAsync_ShouldReturnAllBanksOrderedByName()
    {
        // Act
        var result = await _service.GetAllBanksAsync();
        var banksList = result.ToList();

        // Assert
        banksList.Should().HaveCount(3);
        banksList[0].Name.Should().Be("Банка ДСК");
        banksList[1].Name.Should().Be("Пощенска банка");
        banksList[2].Name.Should().Be("УниКредит Булбанк");
    }

    [Fact]
    public async Task GetBankByIdAsync_WithValidId_ShouldReturnBankWithDeposits()
    {
        // Act
        var result = await _service.GetBankByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.Name.Should().Be("УниКредит Булбанк");
        result.Deposits.Should().NotBeNull();
        result.Deposits.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetBankByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Act
        var result = await _service.GetBankByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateBankAsync_ShouldAddNewBank()
    {
        // Arrange
        var newBank = new Bank
        {
            Name = "Нова банка",
            Logo = "logo.png"
        };

        // Act
        var result = await _service.CreateBankAsync(newBank);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Нова банка");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        var allBanks = await _service.GetAllBanksAsync();
        allBanks.Should().HaveCount(4);
    }

    [Fact]
    public async Task UpdateBankAsync_ShouldUpdateExistingBank()
    {
        // Arrange
        var bank = await _service.GetBankByIdAsync(1);
        bank!.Name = "Обновена банка";

        // Act
        var result = await _service.UpdateBankAsync(bank);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be("Обновена банка");
    }

    [Fact]
    public async Task DeleteBankAsync_WithExistingId_ShouldRemoveBank()
    {
        // Arrange
        var initialCount = (await _service.GetAllBanksAsync()).Count();

        // Act
        await _service.DeleteBankAsync(3);

        // Assert
        var bank = await _service.GetBankByIdAsync(3);
        bank.Should().BeNull();

        var remainingBanks = await _service.GetAllBanksAsync();
        remainingBanks.Should().HaveCount(initialCount - 1);
    }

    [Fact]
    public async Task DeleteBankAsync_WithNonExistingId_ShouldNotThrowException()
    {
        // Act & Assert
        await FluentActions.Invoking(() => _service.DeleteBankAsync(999))
            .Should().NotThrowAsync();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
