using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public interface IAccountType
{
    string Value { get; }
}

public record SavingAccount : IAccountType
{
    private SavingAccount(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static readonly SavingAccount Create = new(AccountConstants.Saving);
}
    
public record CheckingAccount : IAccountType
{
    private CheckingAccount(string value)
    {
        Value = value;
    }

    public string Value { get; }
    public static readonly CheckingAccount Create = new(AccountConstants.Checking);
}

public static class AccountTypeFactory
{
    public static Result<IAccountType> Create(string accountTypeRaw)
    {
        var newTransaction = accountTypeRaw.Trim().ToUpper();
        
        return newTransaction switch
        {
            AccountConstants.Saving => Result<IAccountType>.Success(SavingAccount.Create),
            AccountConstants.Checking => Result<IAccountType>.Success(CheckingAccount.Create),
            _ => Result<IAccountType>.Failure(AccountDomainErrors.AccountTypeNoValid)
        };
    }
}