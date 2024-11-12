using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DevsuAccount.Api.Infrastructure.Persistence.EntityTypeConfiguration;

public class AccountTransactionLogEntityConfiguration : IEntityTypeConfiguration<AccountTransactionLog>
{
    public void Configure(EntityTypeBuilder<AccountTransactionLog> transactionLogConfiguration)
    {
       transactionLogConfiguration.HasKey(tl => tl.Id);
       
       transactionLogConfiguration.Property(tl => tl.DateCreation);
       
       transactionLogConfiguration.Property(tl => tl.TransactionId);
       
       transactionLogConfiguration.Property(tl => tl.PreviousTypeTransaction)
           .HasConversion(new TransactionTypeConverter());
       
       transactionLogConfiguration.Property(tl => tl.NewTypeTransaction)
           .HasConversion(new TransactionTypeConverter());
       
       transactionLogConfiguration.Property(tl => tl.PreviousTransactionValue);
       transactionLogConfiguration.Property(tl => tl.NewTransactionValue);
       
       transactionLogConfiguration.Property(tl => tl.PreviousBalance);
       transactionLogConfiguration.Property(tl => tl.NewBalance);
    }
}