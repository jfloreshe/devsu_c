using DevsuCustomer.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevsuCustomer.Api.Infrastructure.Persistence.EntityTypeConfigurations;

public class PersonEntityConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> personConfiguration)
    {
        personConfiguration.ToTable("People");
        personConfiguration.HasKey(p => p.PersonId); 
        
        personConfiguration.Property(p => p.PersonalIdentifier)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);
        
        personConfiguration.HasIndex(p => p.PersonalIdentifier)
            .IsUnique();
        
        personConfiguration.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode();
        
        personConfiguration.Property(p => p.Gender)
            .HasMaxLength(20);

        personConfiguration.Property(p => p.Age);
        
        personConfiguration.Property(p => p.Address)
            .IsRequired()
            .HasMaxLength(200)
            .IsUnicode();
        
        personConfiguration.Property(p => p.Phone)
            .IsRequired()
            .HasMaxLength(50)
            .IsUnicode(false);
    }
}