using System.ComponentModel.DataAnnotations;
using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public class AccountTransaction
{
    [Key]
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

public static class AccountTransactionUnprocessableErrors
{
    public const string AccountTransactionCreation = "Creacion movimiento";
    public static readonly Error AccountTransactionTypeNoValid = new(
        AccountTransactionCreation,
        $"El tipo de movimiento no es valido; Solo se aceptan los siguientes valores [deposito | retiro]",
        StatusCodes.Status422UnprocessableEntity);
}


public static class AccountTransactionConstants
{
    public const string Withdraw = "RETIRO";
    public const string Deposit = "DEPOSITO";
}