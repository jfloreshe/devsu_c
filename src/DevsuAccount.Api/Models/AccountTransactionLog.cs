namespace DevsuAccount.Api.Models;

public class AccountTransactionLog
{
    public long Id { get; set; }
    public DateTime DateCreation { get; set; }
    public Guid TransactionId { get; set; }
    public IAccountTransactionType PreviousTypeTransaction { get; set; }
    public IAccountTransactionType NewTypeTransaction { get; set; }
    public decimal PreviousTransactionValue { get; set; }
    public decimal NewTransactionValue { get; set; }
    public decimal PreviousBalance { get; set; }
    public decimal NewBalance { get; set; }
    public AccountTransaction Transaction { get; set; }
    
    public AccountTransactionLog(){ }

    public AccountTransactionLog(DateTime dateCreation, Guid transactionId,
        IAccountTransactionType previousTypeTransaction, IAccountTransactionType newTypeTransaction,
        decimal previousTransactionValue, decimal newTransactionValue, decimal previousBalance, decimal newBalance)
    {
        DateCreation = dateCreation;
        TransactionId = transactionId;
        PreviousTypeTransaction = previousTypeTransaction;
        NewTypeTransaction = newTypeTransaction;
        PreviousTransactionValue = previousTransactionValue;
        NewTransactionValue = newTransactionValue;
        PreviousBalance = previousBalance;
        NewBalance = newBalance;
    }
}