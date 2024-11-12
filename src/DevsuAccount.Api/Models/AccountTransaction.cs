using System.ComponentModel.DataAnnotations;
using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public class AccountTransaction
{
    [Key]
    public Guid TransactionId { get; set; }
    public DateTime DateCreation { get; set; }
    public IAccountTransactionType Type { get; set; }
    public decimal TransactionValue { get; set; }
    public decimal Balance { get; set; }
    public long AccountId { get; set; }
    public Account Account { get; set; }
    private AccountTransaction(){ }

    private AccountTransaction(Guid transactionId, DateTime dateCreation, IAccountTransactionType type, decimal transactionValue, decimal balance, long accountId)
    {
        TransactionId =transactionId;
        DateCreation = dateCreation;
        Type = type;
        TransactionValue = transactionValue;
        Balance = balance;
        AccountId = accountId;
    }
    
    public static Result<AccountTransaction> Create(Guid transactionId, DateTime dateCreation, string type, decimal positiveTransactionValue, decimal previousBalance, long accountId)
    {
        if (accountId <= 0)
        {
            return Result<AccountTransaction>.Failure(AccountTransactionDomainErrors.AccountConflict);
        }
        if(positiveTransactionValue <= 0)
        {
            return Result<AccountTransaction>.Failure(AccountTransactionDomainErrors.NoPositiveTransactionValue);
        }

        var accountTransactionTypeResult = AccountTransactionTypeFactory.Create(type);
        if (accountTransactionTypeResult.IsFailure)
        {
            return Result<AccountTransaction>.Failure(accountTransactionTypeResult.Error);
        }
        
        var transactionValue =
            accountTransactionTypeResult.Value is WithdrawTransaction
                ? positiveTransactionValue * -1 
                : positiveTransactionValue;

        var balance = previousBalance + transactionValue;
        if (balance < 0)
        {
            return Result<AccountTransaction>.Failure(AccountTransactionDomainErrors.NotEnoughBalance);
        }
        
        return Result<AccountTransaction>.Success(new AccountTransaction(transactionId, dateCreation, 
            accountTransactionTypeResult.Value, transactionValue, balance, accountId));
    }

}

public static class AccountTransactionDomainErrors
{
    public const string AccountTransactionCreation = "Creacion movimiento";
    public static readonly Error AccountTransactionTypeNoValid = new(
        AccountTransactionCreation,
        AccountTransactionConstants.AccountTransactionTypeNoValidDetails,
        StatusCodes.Status422UnprocessableEntity);
    
    public static readonly Error NoPositiveTransactionValue = new(
        AccountTransactionCreation,
        AccountTransactionConstants.NoPositiveTransactionValueDetails,
        StatusCodes.Status422UnprocessableEntity);
    
    public static Error NotEnoughBalance => new(
        AccountTransactionCreation,
        AccountTransactionConstants.NotEnoughBalanceDetails,
        StatusCodes.Status422UnprocessableEntity);
    
    public static Error AccountConflict => new(
        AccountTransactionCreation,
        AccountTransactionConstants.AccountConflictDetails,
        StatusCodes.Status409Conflict);
}


public static class AccountTransactionConstants
{
    public const string Withdraw = "RETIRO";
    public const string Deposit = "DEPOSITO";

    public const string AccountTransactionTypeNoValidDetails =
        "El tipo de movimiento no es valido; Solo se aceptan los siguientes valores [deposito | retiro]";

    public const string NoPositiveTransactionValueDetails =
        "Solo se aceptan valores mayores a 0, especifique el tipo de movimiento para hacer retiro o deposito";

    public const string NotEnoughBalanceDetails = "Saldo no disponible";
    public const string AccountConflictDetails = "El id de la cuenta no es valido";
}