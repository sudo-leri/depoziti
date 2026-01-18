using BankProducts.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace BankProducts.Core.Data;

/// <summary>
/// Database context for the Bank Products Catalog application.
/// Manages Bank and Deposit entities with their relationships and constraints.
/// </summary>
public class BankProductsDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BankProductsDbContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public BankProductsDbContext(DbContextOptions<BankProductsDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets the Banks DbSet for querying and saving Bank entities.
    /// </summary>
    public DbSet<Bank> Banks => Set<Bank>();

    /// <summary>
    /// Gets the Deposits DbSet for querying and saving Deposit entities.
    /// </summary>
    public DbSet<Deposit> Deposits => Set<Deposit>();

    /// <summary>
    /// Configures the schema and relationships for the database entities.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Bank entity
        modelBuilder.Entity<Bank>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Logo).HasMaxLength(500);
        });

        // Configure Deposit entity with precision for financial fields
        modelBuilder.Entity<Deposit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);

            // Financial amounts with 2 decimal precision (e.g., 12345.67)
            entity.Property(e => e.MinAmount).HasPrecision(18, 2);
            entity.Property(e => e.MaxAmount).HasPrecision(18, 2);

            // Interest rate with 2 decimal precision (e.g., 5.75%)
            entity.Property(e => e.InterestRate).HasPrecision(5, 2);

            // Configure one-to-many relationship with cascade delete
            entity.HasOne(e => e.Bank)
                  .WithMany(b => b.Deposits)
                  .HasForeignKey(e => e.BankId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
