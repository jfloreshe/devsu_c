using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public interface IAccountTransactionType
{
    string Value { get; }
}

public record WithdrawTransaction : IAccountTransactionType
{
    private WithdrawTransaction(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static readonly WithdrawTransaction Create = new(AccountTransactionConstants.Withdraw);
}
    
public record DepositTransaction : IAccountTransactionType
{
    private DepositTransaction(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static readonly DepositTransaction Create = new(AccountTransactionConstants.Deposit);
}

public static class AccountTransactionTypeFactory
{
    public static Result<IAccountTransactionType> Create(string transactionRaw)
    {
        var newTransaction = transactionRaw.Trim().ToUpper();
        
        return newTransaction switch
        {
            AccountTransactionConstants.Withdraw => Result<IAccountTransactionType>.Success(WithdrawTransaction.Create),
            AccountTransactionConstants.Deposit => Result<IAccountTransactionType>.Success(DepositTransaction.Create),
            _ => Result<IAccountTransactionType>.Failure(AccountTransactionUnprocessableErrors.AccountTransactionTypeNoValid)
        };
    }
}