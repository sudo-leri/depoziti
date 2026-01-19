using BankProducts.Core.Models;
using BankProducts.Core.Services;
using BankProducts.Web.Controllers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankProducts.Tests.Controllers;

/// <summary>
/// Unit tests for the DepositsController API.
/// Tests HTTP endpoints, request handling, and response formatting.
/// </summary>
public class DepositsControllerTests
{
    private readonly Mock<IDepositService> _mockService;
    private readonly DepositsController _controller;

    public DepositsControllerTests()
    {
        _mockService = new Mock<IDepositService>();
        _controller = new DepositsController(_mockService.Object);
    }

    [Fact]
    public async Task GetDeposits_WithNoFilters_ShouldReturnAllDeposits()
    {
        // Arrange
        var mockDeposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                Name = "Test Deposit 1",
                InterestRate = 3.5m,
                TermMonths = 12,
                Currency = Currency.BGN,
                MinAmount = 1000,
                Bank = new Bank { Id = 1, Name = "Test Bank" },
                IsActive = true
            },
            new Deposit
            {
                Id = 2,
                Name = "Test Deposit 2",
                InterestRate = 2.8m,
                TermMonths = 6,
                Currency = Currency.EUR,
                MinAmount = 5000,
                Bank = new Bank { Id = 1, Name = "Test Bank" },
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.IsAny<DepositFilter>()))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(null, null, null, null, null, null);

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var deposits = okResult.Value.Should().BeAssignableTo<IEnumerable<DepositDto>>().Subject;
        deposits.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetDeposits_WithBankIdFilter_ShouldPassFilterToService()
    {
        // Arrange
        var mockDeposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                Name = "Test Deposit",
                BankId = 5,
                InterestRate = 3.5m,
                TermMonths = 12,
                Currency = Currency.BGN,
                MinAmount = 1000,
                Bank = new Bank { Id = 5, Name = "Specific Bank" },
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.BankId == 5)))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(5, null, null, null, null, null);

        // Assert
        _mockService.Verify(
            s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.BankId == 5)),
            Times.Once);

        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var deposits = okResult.Value.Should().BeAssignableTo<IEnumerable<DepositDto>>().Subject;
        deposits.Should().HaveCount(1);
        deposits.First().Bank.Id.Should().Be(5);
    }

    [Fact]
    public async Task GetDeposits_WithCurrencyFilter_ShouldPassFilterToService()
    {
        // Arrange
        var mockDeposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                Name = "EUR Deposit",
                InterestRate = 2.5m,
                TermMonths = 12,
                Currency = Currency.EUR,
                MinAmount = 1000,
                Bank = new Bank { Id = 1, Name = "Test Bank" },
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.Currency == Currency.EUR)))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(null, Currency.EUR, null, null, null, null);

        // Assert
        _mockService.Verify(
            s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.Currency == Currency.EUR)),
            Times.Once);
    }

    [Fact]
    public async Task GetDeposits_WithMinTermFilter_ShouldPassFilterToService()
    {
        // Arrange
        var mockDeposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                Name = "Long Term Deposit",
                InterestRate = 4.0m,
                TermMonths = 24,
                Currency = Currency.BGN,
                MinAmount = 1000,
                Bank = new Bank { Id = 1, Name = "Test Bank" },
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.MinTermMonths == 12)))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(null, null, 12, null, null, null);

        // Assert
        _mockService.Verify(
            s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.MinTermMonths == 12)),
            Times.Once);
    }

    [Fact]
    public async Task GetDeposits_WithMaxTermFilter_ShouldPassFilterToService()
    {
        // Arrange
        var mockDeposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                Name = "Short Term Deposit",
                InterestRate = 2.5m,
                TermMonths = 6,
                Currency = Currency.BGN,
                MinAmount = 1000,
                Bank = new Bank { Id = 1, Name = "Test Bank" },
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.MaxTermMonths == 12)))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(null, null, null, 12, null, null);

        // Assert
        _mockService.Verify(
            s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.MaxTermMonths == 12)),
            Times.Once);
    }

    [Fact]
    public async Task GetDeposits_WithMinAmountFilter_ShouldPassFilterToService()
    {
        // Arrange
        var mockDeposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                Name = "Affordable Deposit",
                InterestRate = 3.0m,
                TermMonths = 12,
                Currency = Currency.BGN,
                MinAmount = 1000,
                Bank = new Bank { Id = 1, Name = "Test Bank" },
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.MinAmount == 5000m)))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(null, null, null, null, 5000m, null);

        // Assert
        _mockService.Verify(
            s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.MinAmount == 5000m)),
            Times.Once);
    }

    [Fact]
    public async Task GetDeposits_WithSortByParameter_ShouldPassSortToService()
    {
        // Arrange
        var mockDeposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                Name = "Deposit 1",
                InterestRate = 3.0m,
                TermMonths = 6,
                Currency = Currency.BGN,
                MinAmount = 1000,
                Bank = new Bank { Id = 1, Name = "Bank A" },
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.SortBy == "term")))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(null, null, null, null, null, "term");

        // Assert
        _mockService.Verify(
            s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f => f.SortBy == "term")),
            Times.Once);
    }

    [Fact]
    public async Task GetDeposits_WithMultipleFilters_ShouldPassAllFiltersToService()
    {
        // Arrange
        var mockDeposits = new List<Deposit>();

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f =>
                f.BankId == 1 &&
                f.Currency == Currency.BGN &&
                f.MinTermMonths == 6 &&
                f.MaxTermMonths == 24 &&
                f.MinAmount == 10000m &&
                f.SortBy == "bank")))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(1, Currency.BGN, 6, 24, 10000m, "bank");

        // Assert
        _mockService.Verify(
            s => s.GetFilteredDepositsAsync(It.Is<DepositFilter>(f =>
                f.BankId == 1 &&
                f.Currency == Currency.BGN &&
                f.MinTermMonths == 6 &&
                f.MaxTermMonths == 24 &&
                f.MinAmount == 10000m &&
                f.SortBy == "bank")),
            Times.Once);
    }

    [Fact]
    public async Task GetDeposit_WithValidId_ShouldReturnDeposit()
    {
        // Arrange
        var mockDeposit = new Deposit
        {
            Id = 1,
            Name = "Test Deposit",
            Description = "Test Description",
            InterestRate = 3.5m,
            TermMonths = 12,
            Currency = Currency.BGN,
            MinAmount = 1000,
            MaxAmount = 50000,
            HasCapitalization = true,
            AllowsAdditionalDeposits = false,
            AllowsPartialWithdrawal = false,
            AutoRenewal = true,
            Bank = new Bank { Id = 1, Name = "Test Bank" },
            IsActive = true
        };

        _mockService
            .Setup(s => s.GetDepositByIdAsync(1))
            .ReturnsAsync(mockDeposit);

        // Act
        var result = await _controller.GetDeposit(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var deposit = okResult.Value.Should().BeOfType<DepositDto>().Subject;

        deposit.Id.Should().Be(1);
        deposit.Name.Should().Be("Test Deposit");
        deposit.Description.Should().Be("Test Description");
        deposit.InterestRate.Should().Be(3.5m);
        deposit.TermMonths.Should().Be(12);
        deposit.Currency.Should().Be("BGN");
        deposit.MinAmount.Should().Be(1000);
        deposit.MaxAmount.Should().Be(50000);
        deposit.HasCapitalization.Should().BeTrue();
        deposit.AllowsAdditionalDeposits.Should().BeFalse();
        deposit.AllowsPartialWithdrawal.Should().BeFalse();
        deposit.AutoRenewal.Should().BeTrue();
        deposit.Bank.Id.Should().Be(1);
        deposit.Bank.Name.Should().Be("Test Bank");
    }

    [Fact]
    public async Task GetDeposit_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockService
            .Setup(s => s.GetDepositByIdAsync(999))
            .ReturnsAsync((Deposit?)null);

        // Act
        var result = await _controller.GetDeposit(999);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task GetDeposits_ShouldReturnCorrectDtoFormat()
    {
        // Arrange
        var mockDeposits = new List<Deposit>
        {
            new Deposit
            {
                Id = 1,
                Name = "Test Deposit",
                Description = "Description",
                InterestRate = 3.5m,
                TermMonths = 12,
                Currency = Currency.BGN,
                MinAmount = 1000,
                MaxAmount = null,
                HasCapitalization = true,
                AllowsAdditionalDeposits = true,
                AllowsPartialWithdrawal = false,
                AutoRenewal = true,
                Bank = new Bank { Id = 1, Name = "Test Bank" },
                IsActive = true
            }
        };

        _mockService
            .Setup(s => s.GetFilteredDepositsAsync(It.IsAny<DepositFilter>()))
            .ReturnsAsync(mockDeposits);

        // Act
        var result = await _controller.GetDeposits(null, null, null, null, null, null);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var deposits = okResult.Value.Should().BeAssignableTo<IEnumerable<DepositDto>>().Subject.ToList();

        var deposit = deposits.First();
        deposit.Currency.Should().Be("BGN"); // Currency as string in DTO
        deposit.MaxAmount.Should().BeNull();
        deposit.Bank.Should().NotBeNull();
        deposit.Bank.Name.Should().Be("Test Bank");
    }
}
