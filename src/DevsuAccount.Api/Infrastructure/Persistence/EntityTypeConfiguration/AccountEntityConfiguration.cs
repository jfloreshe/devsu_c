using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevsuAccount.Api.Infrastructure.Persistence.EntityTypeConfiguration;

public class AccountEntityConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> accountConfiguration)
    {
        accountConfiguration.ToTable("Accounts");
        accountConfiguration.HasKey(a => a.AccountId);
        
        accountConfiguration.Property(a => a.AccountNumber);
        accountConfiguration.HasIndex(a => a.AccountNumber)
            .IsUnique();
        
        accountConfiguration.Property(a => a.AccountType);
        accountConfiguration.Property(a => a.OpeningBalance);
        
        accountConfiguration.Property(a => a.CustomerId)
            .IsRequired();
        
        accountConfiguration.HasIndex(a => a.CustomerId);

        accountConfiguration.HasMany(a => a.Transactions)
            .WithOne(t => t.Account)
            .HasForeignKey(t => t.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}