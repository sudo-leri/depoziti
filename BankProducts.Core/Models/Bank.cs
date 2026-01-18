namespace BankProducts.Core.Models;

/// <summary>
/// Represents a financial institution that offers deposit products.
/// </summary>
public class Bank
{
    /// <summary>
    /// Gets or sets the unique identifier for the bank.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the bank.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the URL or path to the bank's logo image.
    /// </summary>
    public string? Logo { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the bank record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the collection of deposit products offered by this bank.
    /// </summary>
    public ICollection<Deposit> Deposits { get; set; } = new List<Deposit>();
}
