namespace BankProducts.Core.Models;

/// <summary>
/// Represents a bank deposit product with its terms, conditions, and features.
/// </summary>
public class Deposit
{
    /// <summary>
    /// Gets or sets the unique identifier for the deposit product.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the bank offering this deposit.
    /// </summary>
    public int BankId { get; set; }

    /// <summary>
    /// Gets or sets the name of the deposit product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description of the deposit product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the minimum deposit amount required.
    /// </summary>
    public decimal MinAmount { get; set; }

    /// <summary>
    /// Gets or sets the maximum deposit amount allowed. Null indicates no maximum.
    /// </summary>
    public decimal? MaxAmount { get; set; }

    /// <summary>
    /// Gets or sets the annual interest rate as a percentage (e.g., 5.5 for 5.5%).
    /// </summary>
    public decimal InterestRate { get; set; }

    /// <summary>
    /// Gets or sets the deposit term in months.
    /// </summary>
    public int TermMonths { get; set; }

    /// <summary>
    /// Gets or sets the currency of the deposit.
    /// </summary>
    public Currency Currency { get; set; } = Currency.BGN;

    /// <summary>
    /// Gets or sets a value indicating whether interest is capitalized (added to principal).
    /// </summary>
    public bool HasCapitalization { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether additional deposits can be made after opening.
    /// </summary>
    public bool AllowsAdditionalDeposits { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether partial withdrawals are allowed during the term.
    /// </summary>
    public bool AllowsPartialWithdrawal { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the deposit automatically renews at maturity.
    /// </summary>
    public bool AutoRenewal { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this deposit product is currently active/available.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets or sets the timestamp when the deposit product was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the timestamp when the deposit product was last updated.
    /// </summary>
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the bank that offers this deposit product.
    /// </summary>
    public Bank Bank { get; set; } = null!;
}
