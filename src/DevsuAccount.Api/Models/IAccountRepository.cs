using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public interface IAccountRepository : IRepository<Account>
{ 
    Task<Account?> FindAccount(string accountNumber, CancellationToken cancellationToken = default);
    void AddAccount(Account newAccount);
    Task<Customer?> FindCustomer(Guid accountCustomerId, CancellationToken cancellationToken = default);
    void UpdateAccount(Account account);
    void DeleteAccount(Account customer);
    void AddTransaction(AccountTransaction newTransactionValue);
    Task<Account?> FindAccountTransaction(Guid accountTransactionId, CancellationToken cancellationToken = default);
}