using BankProducts.Core.Models;

namespace BankProducts.Core.Services;

/// <summary>
/// Service interface for managing deposit products and banks.
/// </summary>
public interface IDepositService
{
    /// <summary>
    /// Retrieves all active deposits ordered by interest rate (highest first).
    /// </summary>
    /// <returns>A collection of active deposit products with their associated banks.</returns>
    Task<IEnumerable<Deposit>> GetAllDepositsAsync();

    /// <summary>
    /// Retrieves active deposits that match the specified filter criteria.
    /// </summary>
    /// <param name="filter">The filter criteria to apply.</param>
    /// <returns>A collection of filtered deposit products with their associated banks.</returns>
    Task<IEnumerable<Deposit>> GetFilteredDepositsAsync(DepositFilter filter);

    /// <summary>
    /// Retrieves a specific deposit by its identifier.
    /// </summary>
    /// <param name="id">The deposit identifier.</param>
    /// <returns>The deposit if found; otherwise, null.</returns>
    Task<Deposit?> GetDepositByIdAsync(int id);

    /// <summary>
    /// Creates a new deposit product.
    /// </summary>
    /// <param name="deposit">The deposit to create.</param>
    /// <returns>The created deposit with updated timestamps.</returns>
    Task<Deposit> CreateDepositAsync(Deposit deposit);

    /// <summary>
    /// Updates an existing deposit product.
    /// </summary>
    /// <param name="deposit">The deposit with updated values.</param>
    /// <returns>The updated deposit.</returns>
    Task<Deposit> UpdateDepositAsync(Deposit deposit);

    /// <summary>
    /// Deletes a deposit product by its identifier.
    /// </summary>
    /// <param name="id">The deposit identifier.</param>
    Task DeleteDepositAsync(int id);

    /// <summary>
    /// Retrieves all banks ordered alphabetically by name.
    /// </summary>
    /// <returns>A collection of all banks.</returns>
    Task<IEnumerable<Bank>> GetAllBanksAsync();

    /// <summary>
    /// Retrieves a specific bank by its identifier, including its deposits.
    /// </summary>
    /// <param name="id">The bank identifier.</param>
    /// <returns>The bank if found; otherwise, null.</returns>
    Task<Bank?> GetBankByIdAsync(int id);

    /// <summary>
    /// Creates a new bank.
    /// </summary>
    /// <param name="bank">The bank to create.</param>
    /// <returns>The created bank with updated timestamp.</returns>
    Task<Bank> CreateBankAsync(Bank bank);

    /// <summary>
    /// Updates an existing bank.
    /// </summary>
    /// <param name="bank">The bank with updated values.</param>
    /// <returns>The updated bank.</returns>
    Task<Bank> UpdateBankAsync(Bank bank);

    /// <summary>
    /// Deletes a bank by its identifier.
    /// </summary>
    /// <param name="id">The bank identifier.</param>
    Task DeleteBankAsync(int id);
}

/// <summary>
/// Represents filtering and sorting criteria for deposit product queries.
/// </summary>
public class DepositFilter
{
    /// <summary>
    /// Gets or sets the bank identifier to filter by. Null means no bank filter.
    /// </summary>
    public int? BankId { get; set; }

    /// <summary>
    /// Gets or sets the currency to filter by. Null means no currency filter.
    /// </summary>
    public Currency? Currency { get; set; }

    /// <summary>
    /// Gets or sets the minimum term in months. Only deposits with terms >= this value are included.
    /// </summary>
    public int? MinTermMonths { get; set; }

    /// <summary>
    /// Gets or sets the maximum term in months. Only deposits with terms <= this value are included.
    /// </summary>
    public int? MaxTermMonths { get; set; }

    /// <summary>
    /// Gets or sets the minimum amount the user has available. Only deposits with MinAmount <= this value are included.
    /// </summary>
    public decimal? MinAmount { get; set; }

    /// <summary>
    /// Gets or sets the field to sort by. Valid values: "term", "minamount", "bank". Default is interest rate.
    /// </summary>
    public string? SortBy { get; set; }

    /// <summary>
    /// Gets or sets whether to sort in descending order. Default sorting is by interest rate descending.
    /// </summary>
    public bool SortDescending { get; set; }
}
