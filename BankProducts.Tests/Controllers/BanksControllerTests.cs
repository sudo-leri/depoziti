using BankProducts.Core.Models;
using BankProducts.Core.Services;
using BankProducts.Web.Controllers.Api;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace BankProducts.Tests.Controllers;

/// <summary>
/// Unit tests for the BanksController API.
/// Tests bank retrieval and DTO conversion.
/// </summary>
public class BanksControllerTests
{
    private readonly Mock<IDepositService> _mockService;
    private readonly BanksController _controller;

    public BanksControllerTests()
    {
        _mockService = new Mock<IDepositService>();
        _controller = new BanksController(_mockService.Object);
    }

    [Fact]
    public async Task GetBanks_ShouldReturnAllBanks()
    {
        // Arrange
        var mockBanks = new List<Bank>
        {
            new Bank { Id = 1, Name = "УниКредит Булбанк" },
            new Bank { Id = 2, Name = "Банка ДСК" },
            new Bank { Id = 3, Name = "Пощенска банка" }
        };

        _mockService
            .Setup(s => s.GetAllBanksAsync())
            .ReturnsAsync(mockBanks);

        // Act
        var result = await _controller.GetBanks();

        // Assert
        result.Should().NotBeNull();
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var banks = okResult.Value.Should().BeAssignableTo<IEnumerable<BankDto>>().Subject;
        banks.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetBanks_ShouldReturnBanksInCorrectFormat()
    {
        // Arrange
        var mockBanks = new List<Bank>
        {
            new Bank { Id = 1, Name = "УниКредит Булбанк", Logo = "logo1.png" },
            new Bank { Id = 2, Name = "Банка ДСК", Logo = "logo2.png" }
        };

        _mockService
            .Setup(s => s.GetAllBanksAsync())
            .ReturnsAsync(mockBanks);

        // Act
        var result = await _controller.GetBanks();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var banks = okResult.Value.Should().BeAssignableTo<IEnumerable<BankDto>>().Subject.ToList();

        banks.Should().HaveCount(2);
        banks[0].Id.Should().Be(1);
        banks[0].Name.Should().Be("УниКредит Булбанк");
        banks[1].Id.Should().Be(2);
        banks[1].Name.Should().Be("Банка ДСК");
    }

    [Fact]
    public async Task GetBanks_WithEmptyResult_ShouldReturnEmptyList()
    {
        // Arrange
        var mockBanks = new List<Bank>();

        _mockService
            .Setup(s => s.GetAllBanksAsync())
            .ReturnsAsync(mockBanks);

        // Act
        var result = await _controller.GetBanks();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var banks = okResult.Value.Should().BeAssignableTo<IEnumerable<BankDto>>().Subject;
        banks.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBanks_ShouldCallServiceOnce()
    {
        // Arrange
        var mockBanks = new List<Bank>
        {
            new Bank { Id = 1, Name = "Test Bank" }
        };

        _mockService
            .Setup(s => s.GetAllBanksAsync())
            .ReturnsAsync(mockBanks);

        // Act
        await _controller.GetBanks();

        // Assert
        _mockService.Verify(s => s.GetAllBanksAsync(), Times.Once);
    }

    [Fact]
    public async Task GetBanks_ShouldMapBankIdCorrectly()
    {
        // Arrange
        var mockBanks = new List<Bank>
        {
            new Bank { Id = 999, Name = "Test Bank" }
        };

        _mockService
            .Setup(s => s.GetAllBanksAsync())
            .ReturnsAsync(mockBanks);

        // Act
        var result = await _controller.GetBanks();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var banks = okResult.Value.Should().BeAssignableTo<IEnumerable<BankDto>>().Subject.ToList();
        banks.First().Id.Should().Be(999);
    }

    [Fact]
    public async Task GetBanks_ShouldMapBankNameCorrectly()
    {
        // Arrange
        var mockBanks = new List<Bank>
        {
            new Bank { Id = 1, Name = "Специална банка с дълго име" }
        };

        _mockService
            .Setup(s => s.GetAllBanksAsync())
            .ReturnsAsync(mockBanks);

        // Act
        var result = await _controller.GetBanks();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var banks = okResult.Value.Should().BeAssignableTo<IEnumerable<BankDto>>().Subject.ToList();
        banks.First().Name.Should().Be("Специална банка с дълго име");
    }

    [Fact]
    public async Task GetBanks_WithMultipleBanks_ShouldMaintainOrder()
    {
        // Arrange
        var mockBanks = new List<Bank>
        {
            new Bank { Id = 1, Name = "Bank A" },
            new Bank { Id = 2, Name = "Bank B" },
            new Bank { Id = 3, Name = "Bank C" }
        };

        _mockService
            .Setup(s => s.GetAllBanksAsync())
            .ReturnsAsync(mockBanks);

        // Act
        var result = await _controller.GetBanks();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var banks = okResult.Value.Should().BeAssignableTo<IEnumerable<BankDto>>().Subject.ToList();

        banks.Should().HaveCount(3);
        banks[0].Name.Should().Be("Bank A");
        banks[1].Name.Should().Be("Bank B");
        banks[2].Name.Should().Be("Bank C");
    }
}
