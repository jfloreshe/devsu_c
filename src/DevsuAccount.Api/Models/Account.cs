using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public class Account : IAggregateRoot
{
    public long AccountId { get; set; }
    public string AccountNumber { get; set; }
    public string Type { get; set; }
    public decimal OpeningBalance { get; set; }
    public bool State { get; set; }
    public Guid CustomerId { get; set; }
    public ICollection<AccountTransaction> Transactions { get;} = new List<AccountTransaction>();
    
    public Account()
    {
        Transactions = new List<AccountTransaction>();
    }

    public Account(string accountNumber, string type, decimal openingBalance, bool state, Guid customerId)
    {
        AccountNumber = accountNumber;
        Type = type;
        OpeningBalance = openingBalance;
        State = state;
        CustomerId = customerId;
    }
}