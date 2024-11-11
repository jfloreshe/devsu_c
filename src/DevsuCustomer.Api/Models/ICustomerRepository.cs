using DevsuCustomer.Api.Models.Primitives;

namespace DevsuCustomer.Api.Models;

public interface ICustomerRepository : IRepository<Customer>
{
    void AddCustomer(Customer newCustomer);
    Task<Customer?> FindCustomer(Guid customerId, CancellationToken cancellationToken = default);
    Task<int> SaveEntities(CancellationToken cancellationToken = default);
}