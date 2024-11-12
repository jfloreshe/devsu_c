using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public class Account : IAggregateRoot
{
    public long AccountId { get; set; }
    public string AccountNumber { get; set; }
    public IAccountType AccountType { get; set; }
    public decimal OpeningBalance { get; set; }
    public bool State { get; set; }
    public Guid CustomerId { get; set; }
    public ICollection<AccountTransaction> Transactions { get;} = new List<AccountTransaction>();
    
    private Account()
    {
        Transactions = new List<AccountTransaction>();
    }

    private Account(string accountNumber, IAccountType accountType, decimal openingBalance, bool state, Guid customerId)
    {
        AccountNumber = accountNumber;
        AccountType = accountType;
        OpeningBalance = openingBalance;
        State = state;
        CustomerId = customerId;
    }
    
    public static Result<Account> Create(string accountNumber, string accountTypeRaw, decimal openingBalance, bool state, Guid customerId)
    {
        var accountTypeResult = AccountTypeFactory.Create(accountTypeRaw);
        if (accountTypeResult.IsFailure)
        {
            return Result<Account>.Failure(accountTypeResult.Error);
        }
        var newAccount = new Account(accountNumber, accountTypeResult.Value, openingBalance, state, customerId);
        return Result<Account>.Success(newAccount);
    }

    public Result<AccountTransaction> AddNewTransaction(string transactionType, decimal transactionValue)
    {
        //Account is the owner of transaction and must create it
        
        var previousBalance = Transactions.Count == 0
            ? OpeningBalance
            : Transactions.OrderByDescending(t => t.DateCreation)
                .First()
                .Balance;

        var newTransaction = AccountTransaction.Create(Guid.NewGuid(), DateTime.Now, transactionType, transactionValue, previousBalance, AccountId);
        if (newTransaction.IsFailure)
        {
            return Result<AccountTransaction>.Failure(newTransaction.Error);
        }
        
        Transactions.Add(newTransaction.Value);
        
        return Result<AccountTransaction>.Success(newTransaction.Value);
    }
}

public static class AccountDomainErrors
{
    public const string AccountTransactionCreation = "Creacion cuenta";
    public static readonly Error AccountTypeNoValid = new(
        AccountTransactionCreation,
        AccountConstants.AccountTypeNoValidDetails,
        StatusCodes.Status422UnprocessableEntity);
}

public static class AccountConstants
{
    public const string Saving = "AHORROS";
    public const string Checking = "CORRIENTE";
    
    public const string AccountTypeNoValidDetails =
        "El tipo de cuenta no es valido; Solo se aceptan los siguientes valores [ahorros | corriente]";
}