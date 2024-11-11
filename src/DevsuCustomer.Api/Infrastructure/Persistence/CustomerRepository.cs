using DevsuCustomer.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevsuCustomer.Api.Infrastructure.Persistence;

public class CustomerRepository : ICustomerRepository
{
    private readonly CustomerDbContext _ctx;

    public CustomerRepository(CustomerDbContext ctx)
    {
        _ctx = ctx;
    }

    public void AddCustomer(Customer newCustomer)
    {
        _ctx.Customers.Add(newCustomer);
    }

    public Task<Customer?> FindCustomer(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = _ctx.Customers
            .Where(c => c.CustomerId == customerId)
            .FirstOrDefaultAsync(cancellationToken);

        return customer;
    }

    public Task<int> SaveEntities(CancellationToken cancellationToken = default)
    {
        return _ctx.SaveChangesAsync(cancellationToken);
    }
}