using BankProducts.Core.Models;
using BankProducts.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankProducts.Web.Controllers.Api;

/// <summary>
/// API controller for managing deposit products with filtering and search capabilities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DepositsController : ControllerBase
{
    private readonly IDepositService _depositService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DepositsController"/> class.
    /// </summary>
    /// <param name="depositService">The deposit service.</param>
    public DepositsController(IDepositService depositService)
    {
        _depositService = depositService;
    }

    /// <summary>
    /// Retrieves deposits with optional filtering and sorting.
    /// </summary>
    /// <param name="bankId">Filter by bank identifier.</param>
    /// <param name="currency">Filter by currency (BGN, EUR, USD).</param>
    /// <param name="minTerm">Minimum term in months.</param>
    /// <param name="maxTerm">Maximum term in months.</param>
    /// <param name="minAmount">Minimum amount available (returns deposits with MinAmount less than or equal to this).</param>
    /// <param name="sortBy">Field to sort by: "term", "minamount", "bank", or default (interest rate).</param>
    /// <returns>A filtered and sorted collection of deposit products.</returns>
    /// <response code="200">Returns the filtered list of deposits.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DepositDto>>> GetDeposits(
        [FromQuery] int? bankId,
        [FromQuery] Currency? currency,
        [FromQuery] int? minTerm,
        [FromQuery] int? maxTerm,
        [FromQuery] decimal? minAmount,
        [FromQuery] string? sortBy)
    {
        var filter = new DepositFilter
        {
            BankId = bankId,
            Currency = currency,
            MinTermMonths = minTerm,
            MaxTermMonths = maxTerm,
            MinAmount = minAmount,
            SortBy = sortBy
        };

        var deposits = await _depositService.GetFilteredDepositsAsync(filter);
        return Ok(deposits.Select(d => new DepositDto(d)));
    }

    /// <summary>
    /// Retrieves a specific deposit by its identifier.
    /// </summary>
    /// <param name="id">The deposit identifier.</param>
    /// <returns>The deposit details if found.</returns>
    /// <response code="200">Returns the deposit details.</response>
    /// <response code="404">If the deposit is not found.</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<DepositDto>> GetDeposit(int id)
    {
        var deposit = await _depositService.GetDepositByIdAsync(id);
        if (deposit == null)
            return NotFound();

        return Ok(new DepositDto(deposit));
    }
}

/// <summary>
/// Data transfer object for deposit products.
/// </summary>
/// <param name="Id">The deposit identifier.</param>
/// <param name="Name">The deposit product name.</param>
/// <param name="Description">The deposit description.</param>
/// <param name="MinAmount">The minimum deposit amount required.</param>
/// <param name="MaxAmount">The maximum deposit amount allowed (null if no limit).</param>
/// <param name="InterestRate">The annual interest rate as a percentage.</param>
/// <param name="TermMonths">The deposit term in months.</param>
/// <param name="Currency">The currency code (BGN, EUR, USD).</param>
/// <param name="HasCapitalization">Whether interest is capitalized.</param>
/// <param name="AllowsAdditionalDeposits">Whether additional deposits are allowed.</param>
/// <param name="AllowsPartialWithdrawal">Whether partial withdrawals are allowed.</param>
/// <param name="AutoRenewal">Whether the deposit automatically renews at maturity.</param>
/// <param name="Bank">The bank offering this deposit.</param>
public record DepositDto(
    int Id,
    string Name,
    string? Description,
    decimal MinAmount,
    decimal? MaxAmount,
    decimal InterestRate,
    int TermMonths,
    string Currency,
    bool HasCapitalization,
    bool AllowsAdditionalDeposits,
    bool AllowsPartialWithdrawal,
    bool AutoRenewal,
    BankDto Bank
)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DepositDto"/> class from a Deposit entity.
    /// </summary>
    /// <param name="d">The deposit entity.</param>
    public DepositDto(Deposit d) : this(
        d.Id,
        d.Name,
        d.Description,
        d.MinAmount,
        d.MaxAmount,
        d.InterestRate,
        d.TermMonths,
        d.Currency.ToString(),
        d.HasCapitalization,
        d.AllowsAdditionalDeposits,
        d.AllowsPartialWithdrawal,
        d.AutoRenewal,
        new BankDto(d.Bank.Id, d.Bank.Name)
    ) { }
}

/// <summary>
/// Data transfer object for banks.
/// </summary>
/// <param name="Id">The bank identifier.</param>
/// <param name="Name">The bank name.</param>
public record BankDto(int Id, string Name);
