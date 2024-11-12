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
    
    public static Result<Account> Create(string accountNumber, string accountTypeRaw, decimal openingBalance,
        bool state, Guid customerId)
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

        var newTransaction = AccountTransaction.Create(Guid.NewGuid(), DateTime.Now, transactionType, transactionValue,
            previousBalance, AccountId);
        if (newTransaction.IsFailure)
        {
            return Result<AccountTransaction>.Failure(newTransaction.Error);
        }

        newTransaction.Value.Logs.Add(
            new AccountTransactionLog(
            dateCreation: DateTime.Now, 
            transactionId: newTransaction.Value.TransactionId,
            previousTypeTransaction: newTransaction.Value.Type,
            newTypeTransaction: newTransaction.Value.Type,
            previousTransactionValue: newTransaction.Value.TransactionValue,
            newTransactionValue: newTransaction.Value.TransactionValue,
            previousBalance: newTransaction.Value.Balance,
            newBalance: newTransaction.Value.Balance));
            
        Transactions.Add(newTransaction.Value);
        
        return Result<AccountTransaction>.Success(newTransaction.Value);
    }

    public Result<AccountTransaction> UpdateTransaction(Guid accountTransactionId, string newTransactionType,
        decimal newPositiveTransactionValue)
    {
        var newTransactionTypeResult = AccountTransactionTypeFactory.Create(newTransactionType);
        if(newTransactionTypeResult.IsFailure)
        {
            return Result<AccountTransaction>.Failure(newTransactionTypeResult.Error);
        }
        
        if(newPositiveTransactionValue <= 0)
        {
            return Result<AccountTransaction>.Failure(AccountTransactionDomainErrors.NoPositiveTransactionValue);
        }

        var tempTransactions = Transactions.OrderBy(t => t.DateCreation).ToList();
        
        var (rootAccountTransaction, rootIndex) = tempTransactions
            .Select((transaction, index) => (transaction, index))
            .First(tuple => tuple.transaction.TransactionId == accountTransactionId);
        
        var rootTransactionPreviousBalance = rootIndex == 0
            ? OpeningBalance
            : tempTransactions[rootIndex - 1].Balance;

        var newBalanceResult = AccountTransaction.CalculateNewBalance(
            previousTransactionBalance: rootTransactionPreviousBalance,
            currentTransactionType: newTransactionTypeResult.Value,
            currentTransactionPositiveValue: newPositiveTransactionValue,
            calledFrom: AccountDomainErrors.UpdateAccountTransaction);
        
        if(newBalanceResult.IsFailure)
        {
            return Result<AccountTransaction>.Failure(AccountDomainErrors.InconsistentBalance(
                rootAccountTransaction.TransactionId,
                rootAccountTransaction.TransactionId, 
                newBalanceResult.Error.Description ?? string.Empty));
        }

        var newTransactionValue = newBalanceResult.Value - rootTransactionPreviousBalance;
        
        var transactionUpdateLog = new AccountTransactionLog(
            dateCreation: DateTime.Now, 
            transactionId: rootAccountTransaction.TransactionId,
            previousTypeTransaction: rootAccountTransaction.Type,
            newTypeTransaction: newTransactionTypeResult.Value,
            previousTransactionValue: rootAccountTransaction.TransactionValue,
            newTransactionValue: newTransactionValue,
            previousBalance: rootAccountTransaction.Balance,
            newBalance: newBalanceResult.Value);

        rootAccountTransaction.Logs.Add(transactionUpdateLog);
        rootAccountTransaction.TransactionValue = transactionUpdateLog.NewTransactionValue;
        rootAccountTransaction.Type = transactionUpdateLog.NewTypeTransaction;
        rootAccountTransaction.Balance = transactionUpdateLog.NewBalance;
        
            
        for (var i = rootIndex + 1 ; i < tempTransactions.Count; i++)
        {
            var previousCurrentTransaction = tempTransactions[i - 1];
            var currentTransaction = tempTransactions[i];
            var currentTransactionPositiveValue = currentTransaction.Type is WithdrawTransaction
                ? currentTransaction.TransactionValue * -1
                : currentTransaction.TransactionValue;
                
            var currentTransactionNewBalanceResult = AccountTransaction.CalculateNewBalance(
                previousTransactionBalance: previousCurrentTransaction.Balance,
                currentTransactionType: currentTransaction.Type,
                currentTransactionPositiveValue: currentTransactionPositiveValue,
                calledFrom: AccountDomainErrors.UpdateAccountTransaction);
        
            if(currentTransactionNewBalanceResult.IsFailure)
            {
                return Result<AccountTransaction>.Failure(AccountDomainErrors.InconsistentBalance(
                    rootAccountTransaction.TransactionId,
                    currentTransaction.TransactionId, 
                    currentTransactionNewBalanceResult.Error.Description ?? string.Empty));
            }
            
            var currentTransactionUpdateLog = new AccountTransactionLog(
                dateCreation: DateTime.Now, 
                transactionId: currentTransaction.TransactionId,
                previousTypeTransaction: currentTransaction.Type,
                newTypeTransaction: currentTransaction.Type,
                previousTransactionValue: currentTransaction.TransactionValue,
                newTransactionValue: currentTransaction.TransactionValue,
                previousBalance: currentTransaction.Balance,
                newBalance: currentTransactionNewBalanceResult.Value);
        
            currentTransaction.Logs.Add(transactionUpdateLog);
            currentTransaction.Balance = currentTransactionUpdateLog.NewBalance;
        }

        return Result<AccountTransaction>.Success(rootAccountTransaction);
    }
}

public static class AccountDomainErrors
{
    public const string AccountCreation = "Creacion cuenta";
    public const string UpdateAccountTransaction = "Actualizacion movimiento";
    
    public static readonly Error AccountTypeNoValid = new(
        AccountCreation,
        AccountConstants.AccountTypeNoValidDetails,
        StatusCodes.Status422UnprocessableEntity);
    
    public static Error InconsistentBalance(Guid seedTransactionId, Guid inconsistentTransactionId, string innerError) => new(
        UpdateAccountTransaction,
        $"Se genera inconsistencia de datos en los saldos a partir de intentar actualizar el movimiento {seedTransactionId}," +
        $"movimiento fallido {inconsistentTransactionId}, {innerError}",
        StatusCodes.Status409Conflict);
}

public static class AccountConstants
{
    public const string Saving = "AHORROS";
    public const string Checking = "CORRIENTE";
    
    public const string AccountTypeNoValidDetails =
        "El tipo de cuenta no es valido; Solo se aceptan los siguientes valores [ahorros | corriente]";
}