using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevsuAccount.Api.Infrastructure.Persistence.EntityTypeConfiguration;

public class AccountTransactionEntityConfiguration : IEntityTypeConfiguration<AccountTransaction>
{
    public void Configure(EntityTypeBuilder<AccountTransaction> transactionConfiguration)
    {
        transactionConfiguration.ToTable("Transactions");

        transactionConfiguration.HasKey(t => t.TransactionId);
        transactionConfiguration.Property(t => t.DateCreation);
        transactionConfiguration.Property(t => t.TransactionValue);
        transactionConfiguration.Property(t => t.Balance);

        transactionConfiguration.Ignore(t => t.Account);
        transactionConfiguration.Property(t => t.Type)
            .IsRequired()
            .HasConversion(new TransactionTypeConverter());
    }
}

public class TransactionTypeConverter : ValueConverter<IAccountTransactionType, string>
{
    public TransactionTypeConverter() : base(
        vObject => vObject.Value,
        vPrimitive => AccountTransactionTypeFactory.Create(vPrimitive).Value)
    {}
}