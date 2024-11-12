using Devsu.Shared.Primitives;

namespace DevsuCustomer.Api.Models;

public interface ICustomerRepository : IRepository<Customer>
{
    void AddCustomer(Customer newCustomer);
    void UpdateCustomer(Customer customer);
    void DeleteCustomer(Customer customer);
    Task<Customer?> FindCustomer(Guid customerId, CancellationToken cancellationToken = default);
    Task<Customer?> FindCustomer(string personalIdentifier, CancellationToken cancellationToken = default);
    Task<int> SaveEntities(CancellationToken cancellationToken = default);
}