using BankProducts.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankProducts.Web.Controllers.Api;

/// <summary>
/// API controller for managing bank entities.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class BanksController : ControllerBase
{
    private readonly IDepositService _depositService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BanksController"/> class.
    /// </summary>
    /// <param name="depositService">The deposit service.</param>
    public BanksController(IDepositService depositService)
    {
        _depositService = depositService;
    }

    /// <summary>
    /// Retrieves all banks.
    /// </summary>
    /// <returns>A collection of banks ordered alphabetically by name.</returns>
    /// <response code="200">Returns the list of banks.</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BankDto>>> GetBanks()
    {
        var banks = await _depositService.GetAllBanksAsync();
        return Ok(banks.Select(b => new BankDto(b.Id, b.Name)));
    }
}
