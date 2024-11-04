using CloudSales.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudSales.Persistence.Database;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<License> Licenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>()
            .ToTable("Accounts")
            .HasKey(x => x.AccountId);

        modelBuilder.Entity<Account>().Property(x => x.UserName).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Account>().Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Account>().Property(x => x.LastName).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Account>()
            .HasMany(x => x.Licenses)
            .WithOne(x => x.Account)
            .HasForeignKey(x => x.AccountId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>()
            .ToTable("Customers")
            .HasKey(x => x.CustomerId);

        modelBuilder.Entity<Customer>().Property(x => x.UserName).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Customer>().Property(x => x.CustomerName).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<Customer>()
            .HasMany(x => x.Accounts)
            .WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Customer>().HasIndex(x => x.UserName).IsUnique();

        modelBuilder.Entity<License>()
            .ToTable("Licenses")
            .HasKey(x => new { x.AccountId, x.ServiceId });

        modelBuilder.Entity<License>().Property(x => x.State).IsRequired().HasConversion<int>();
        modelBuilder.Entity<License>().Property(x => x.ExpiryDate).IsRequired().HasColumnType("datetime");
        modelBuilder.Entity<License>().Property(x => x.ServiceName).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<License>().Property(x => x.Price).IsRequired().HasPrecision(10, 2);
        modelBuilder.Entity<License>().Property(x => x.Quantity).IsRequired();
        modelBuilder.Entity<License>()
            .HasOne(x => x.Account)
            .WithMany(x => x.Licenses)
            .HasForeignKey(x => x.AccountId);
    }
}
