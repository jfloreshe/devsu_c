namespace DevsuAccount.Api.Models;

public interface ICustomerRepository
{
    Task<AccountCustomer?> FindCustomer(Guid customerId, CancellationToken cancellationToken = default);
    Task InsertCustomer(AccountCustomer accountCustomer, CancellationToken cancellationToken = default);
    Task UpdateCustomer(AccountCustomer accountCustomer, CancellationToken cancellationToken = default);
    void DeleteCustomer(AccountCustomer customer);
}