using Devsu.Shared.Primitives;

namespace DevsuAccount.Api.Models;

public interface IAccountRepository : IRepository<Account>
{ 
    Task<Account?> FindAccountAsync(string accountNumber, CancellationToken cancellationToken = default);
    void AddAccount(Account newAccount);
    Task<Customer?> FindCustomerAsync(Guid accountCustomerId, CancellationToken cancellationToken = default);
    void UpdateAccount(Account account);
}