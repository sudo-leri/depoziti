using BankProducts.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BankProducts.Core.Data;

/// <summary>
/// Provides database seeding functionality with sample Bulgarian bank deposit data.
/// </summary>
public static class DataSeeder
{
    /// <summary>
    /// Seeds the database with sample banks and deposit products if no data exists.
    /// </summary>
    /// <param name="context">The database context to seed.</param>
    /// <remarks>
    /// This method will:
    /// 1. Ensure the database is created
    /// 2. Check if banks already exist (skip seeding if they do)
    /// 3. Add 6 Bulgarian banks
    /// 4. Add 10 sample deposit products with various terms and features
    /// </remarks>
    public static async Task SeedAsync(BankProductsDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        // Skip seeding if data already exists
        if (await context.Banks.AnyAsync())
            return;

        var banks = new List<Bank>
        {
            new() { Name = "УниКредит Булбанк" },
            new() { Name = "Банка ДСК" },
            new() { Name = "Пощенска банка" },
            new() { Name = "Райфайзенбанк" },
            new() { Name = "ОББ" },
            new() { Name = "Първа инвестиционна банка" }
        };

        context.Banks.AddRange(banks);
        await context.SaveChangesAsync();

        var deposits = new List<Deposit>
        {
            // УниКредит Булбанк
            new()
            {
                BankId = banks[0].Id,
                Name = "Стандартен депозит BGN",
                Description = "Срочен депозит в лева с фиксирана лихва",
                MinAmount = 500,
                InterestRate = 2.5m,
                TermMonths = 12,
                Currency = Currency.BGN,
                HasCapitalization = false,
                AutoRenewal = true
            },
            new()
            {
                BankId = banks[0].Id,
                Name = "Премиум депозит",
                Description = "Депозит с по-висока лихва за суми над 10000 лв",
                MinAmount = 10000,
                InterestRate = 3.2m,
                TermMonths = 12,
                Currency = Currency.BGN,
                HasCapitalization = true,
                AutoRenewal = true
            },
            // Банка ДСК
            new()
            {
                BankId = banks[1].Id,
                Name = "ДСК Директ депозит",
                Description = "Депозит с онлайн управление",
                MinAmount = 100,
                InterestRate = 2.8m,
                TermMonths = 6,
                Currency = Currency.BGN,
                HasCapitalization = false,
                AllowsAdditionalDeposits = true
            },
            new()
            {
                BankId = banks[1].Id,
                Name = "ДСК Спестовен депозит EUR",
                Description = "Депозит в евро за дългосрочни спестявания",
                MinAmount = 500,
                InterestRate = 1.8m,
                TermMonths = 24,
                Currency = Currency.EUR,
                HasCapitalization = true,
                AutoRenewal = true
            },
            // Пощенска банка
            new()
            {
                BankId = banks[2].Id,
                Name = "Гъвкав депозит",
                Description = "Депозит с възможност за частично теглене",
                MinAmount = 200,
                InterestRate = 2.2m,
                TermMonths = 12,
                Currency = Currency.BGN,
                AllowsPartialWithdrawal = true,
                AllowsAdditionalDeposits = true
            },
            // Райфайзенбанк
            new()
            {
                BankId = banks[3].Id,
                Name = "Райфайзен Стандарт",
                Description = "Класически срочен депозит",
                MinAmount = 1000,
                InterestRate = 2.6m,
                TermMonths = 12,
                Currency = Currency.BGN,
                HasCapitalization = false
            },
            new()
            {
                BankId = banks[3].Id,
                Name = "Райфайзен USD депозит",
                Description = "Депозит в щатски долари",
                MinAmount = 500,
                InterestRate = 3.5m,
                TermMonths = 12,
                Currency = Currency.USD,
                HasCapitalization = true
            },
            // ОББ
            new()
            {
                BankId = banks[4].Id,
                Name = "ОББ Растящ депозит",
                Description = "Депозит с нарастваща лихва",
                MinAmount = 500,
                InterestRate = 2.9m,
                TermMonths = 18,
                Currency = Currency.BGN,
                HasCapitalization = true,
                AutoRenewal = true
            },
            // Първа инвестиционна банка
            new()
            {
                BankId = banks[5].Id,
                Name = "Fibank Промо депозит",
                Description = "Промоционален депозит с атрактивна лихва",
                MinAmount = 1000,
                InterestRate = 3.8m,
                TermMonths = 6,
                Currency = Currency.BGN,
                HasCapitalization = false
            },
            new()
            {
                BankId = banks[5].Id,
                Name = "Fibank Дългосрочен",
                Description = "Депозит за 3 години с висока лихва",
                MinAmount = 5000,
                InterestRate = 4.2m,
                TermMonths = 36,
                Currency = Currency.BGN,
                HasCapitalization = true,
                AutoRenewal = true
            }
        };

        context.Deposits.AddRange(deposits);
        await context.SaveChangesAsync();
    }
}
