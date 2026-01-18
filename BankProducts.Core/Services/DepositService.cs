using BankProducts.Core.Data;
using BankProducts.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BankProducts.Core.Services;

/// <summary>
/// Service implementation for managing deposit products and banks with Entity Framework Core.
/// </summary>
public class DepositService : IDepositService
{
    private readonly BankProductsDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="DepositService"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DepositService(BankProductsDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Deposit>> GetAllDepositsAsync()
    {
        return await _context.Deposits
            .Include(d => d.Bank)
            .Where(d => d.IsActive)
            .OrderByDescending(d => d.InterestRate)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Deposit>> GetFilteredDepositsAsync(DepositFilter filter)
    {
        var query = _context.Deposits
            .Include(d => d.Bank)
            .Where(d => d.IsActive)
            .AsQueryable();

        // Apply filtering criteria
        if (filter.BankId.HasValue)
            query = query.Where(d => d.BankId == filter.BankId.Value);

        if (filter.Currency.HasValue)
            query = query.Where(d => d.Currency == filter.Currency.Value);

        if (filter.MinTermMonths.HasValue)
            query = query.Where(d => d.TermMonths >= filter.MinTermMonths.Value);

        if (filter.MaxTermMonths.HasValue)
            query = query.Where(d => d.TermMonths <= filter.MaxTermMonths.Value);

        if (filter.MinAmount.HasValue)
            query = query.Where(d => d.MinAmount <= filter.MinAmount.Value);

        // Apply sorting - default is by interest rate descending (highest first)
        query = filter.SortBy?.ToLower() switch
        {
            "term" => filter.SortDescending
                ? query.OrderByDescending(d => d.TermMonths)
                : query.OrderBy(d => d.TermMonths),
            "minamount" => filter.SortDescending
                ? query.OrderByDescending(d => d.MinAmount)
                : query.OrderBy(d => d.MinAmount),
            "bank" => filter.SortDescending
                ? query.OrderByDescending(d => d.Bank.Name)
                : query.OrderBy(d => d.Bank.Name),
            _ => filter.SortDescending
                ? query.OrderBy(d => d.InterestRate)
                : query.OrderByDescending(d => d.InterestRate)
        };

        return await query.ToListAsync();
    }

    public async Task<Deposit?> GetDepositByIdAsync(int id)
    {
        return await _context.Deposits
            .Include(d => d.Bank)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<Deposit> CreateDepositAsync(Deposit deposit)
    {
        deposit.CreatedAt = DateTime.UtcNow;
        deposit.UpdatedAt = DateTime.UtcNow;
        _context.Deposits.Add(deposit);
        await _context.SaveChangesAsync();
        return deposit;
    }

    public async Task<Deposit> UpdateDepositAsync(Deposit deposit)
    {
        deposit.UpdatedAt = DateTime.UtcNow;
        _context.Deposits.Update(deposit);
        await _context.SaveChangesAsync();
        return deposit;
    }

    public async Task DeleteDepositAsync(int id)
    {
        var deposit = await _context.Deposits.FindAsync(id);
        if (deposit != null)
        {
            _context.Deposits.Remove(deposit);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Bank>> GetAllBanksAsync()
    {
        return await _context.Banks.OrderBy(b => b.Name).ToListAsync();
    }

    public async Task<Bank?> GetBankByIdAsync(int id)
    {
        return await _context.Banks
            .Include(b => b.Deposits)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Bank> CreateBankAsync(Bank bank)
    {
        bank.CreatedAt = DateTime.UtcNow;
        _context.Banks.Add(bank);
        await _context.SaveChangesAsync();
        return bank;
    }

    public async Task<Bank> UpdateBankAsync(Bank bank)
    {
        _context.Banks.Update(bank);
        await _context.SaveChangesAsync();
        return bank;
    }

    public async Task DeleteBankAsync(int id)
    {
        var bank = await _context.Banks.FindAsync(id);
        if (bank != null)
        {
            _context.Banks.Remove(bank);
            await _context.SaveChangesAsync();
        }
    }
}
