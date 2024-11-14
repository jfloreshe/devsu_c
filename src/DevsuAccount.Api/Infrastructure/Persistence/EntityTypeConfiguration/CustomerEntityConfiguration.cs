using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevsuAccount.Api.Infrastructure.Persistence.EntityTypeConfiguration;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<AccountCustomer>
{
    public void Configure(EntityTypeBuilder<AccountCustomer> customerConfiguration)
    {
        customerConfiguration.ToTable("Customers");
        customerConfiguration.HasKey(c => c.CustomerId);
        
        customerConfiguration.Property(c => c.Name)
            .HasMaxLength(200)
            .IsUnicode();

        customerConfiguration.Property(c => c.State)
            .HasDefaultValue(true);
    }
}