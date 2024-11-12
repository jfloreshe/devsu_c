using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevsuAccount.Api.Infrastructure.Persistence.EntityTypeConfiguration;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> customerConfiguration)
    {
        customerConfiguration.ToTable("Customers");
        customerConfiguration.HasKey(c => c.CustomerId);
        
        customerConfiguration.Property(c => c.Name)
            .HasMaxLength(200)
            .IsUnicode();
    }
}