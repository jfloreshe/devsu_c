using DevsuCustomer.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevsuCustomer.Api.Infrastructure.Persistence.EntityTypeConfigurations;

public class CustomerEntityConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> customerConfiguration)
    {
        customerConfiguration.ToTable("Customers");
        customerConfiguration.HasBaseType<Person>();
        
        customerConfiguration.Property(c => c.CustomerId)
            .IsRequired();
        customerConfiguration.HasIndex(c => c.CustomerId)
            .IsUnique();

        customerConfiguration.Property(c => c.Password)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);

        customerConfiguration.Property(c => c.State)
            .IsRequired();
    }
}