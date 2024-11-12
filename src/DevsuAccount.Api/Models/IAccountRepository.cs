using Devsu.Shared.Primitives;
using DevsuAccount.Api.Features.Account;

namespace DevsuAccount.Api.Models;

public interface IAccountRepository : IRepository<Account>
{ 
    Task<Account?> FindAccount(string accountNumber, CancellationToken cancellationToken = default);
    void AddAccount(Account newAccount);
    Task<Customer?> FindCustomer(Guid accountCustomerId, CancellationToken cancellationToken = default);
    void UpdateAccount(Account account);
    void DeleteAccount(Account customer);
    void AddTransaction(AccountTransaction newTransactionValue);
    Task<Account?> FindAccount(Guid accountTransactionId, CancellationToken cancellationToken = default);
    void DeleteAccountTransaction(AccountTransaction accountTransaction);
    Task<GetAccountStateReportResult> GetAccountState(Guid requestCustomerId, DateTime initialDate,
        DateTime finalDate, int page, int size, CancellationToken cancellationToken = default);
}