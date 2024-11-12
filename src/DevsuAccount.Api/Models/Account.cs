using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public class Account : IAggregateRoot
{
    public long AccountId { get; set; }
    public string AccountNumber { get; set; }
    public string AccountType { get; set; }
    public decimal OpeningBalance { get; set; }
    public bool State { get; set; }
    public Guid CustomerId { get; set; }
    public ICollection<AccountTransaction> Transactions { get;} = new List<AccountTransaction>();
    
    public Account()
    {
        Transactions = new List<AccountTransaction>();
    }

    public Account(string accountNumber, string accountType, decimal openingBalance, bool state, Guid customerId)
    {
        AccountNumber = accountNumber;
        AccountType = accountType;
        OpeningBalance = openingBalance;
        State = state;
        CustomerId = customerId;
    }

    public Result<AccountTransaction> AddNewTransaction(string transactionType, decimal transactionValue)
    {
        //Account is the owner of transaction and must create it
        
        var previousBalance = Transactions.Count == 0
            ? OpeningBalance
            : Transactions.OrderByDescending(t => t.DateCreation)
                .First()
                .Balance;

        var newTransaction = new AccountTransaction(Guid.NewGuid(), DateTime.Now, transactionType, transactionValue, previousBalance, AccountId);
        //We need to find possible errors with the newTransaction, so it should be a result
        
        Transactions.Add(newTransaction);
        
        return Result<AccountTransaction>.Success(newTransaction);
    }
}