namespace DevsuAccount.Api.Models;

public class AccountTransaction
{
    public Guid TransactionId { get; set; }
    public DateTime DateCreation { get; set; }
    public string Type { get; set; }
    public decimal TransactionValue { get; set; }
    public decimal Balance { get; set; }
    public long AccountId { get; set; }
    public Account Account { get; set; }
    public AccountTransaction(){ }

    public AccountTransaction(Guid transactionId, DateTime dateCreation, string type, decimal transactionValue, decimal balance, long accountId)
    {
        TransactionId =transactionId;
        DateCreation = dateCreation;
        Type = type;
        TransactionValue = transactionValue;
        Balance = balance;
        AccountId = accountId;
    }

}