using DevsuAccount.Api.Infrastructure.Persistence.EntityTypeConfiguration;
using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevsuAccount.Api.Infrastructure.Persistence;

public class AccountDbContext : DbContext
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountTransaction> Transactions { get; set; }
    public DbSet<Customer> Customers { get; set; }
  
    public AccountDbContext(){}
    public AccountDbContext(DbContextOptions<AccountDbContext> options) : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AccountEntityConfiguration());
        modelBuilder.ApplyConfiguration(new AccountTransactionEntityConfiguration());
        modelBuilder.ApplyConfiguration(new CustomerEntityConfiguration());
    }
}