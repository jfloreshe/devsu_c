using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevsuAccount.Api.Infrastructure.Persistence;

public class CustomerRepository : ICustomerRepository
{
    private readonly AccountDbContext _ctx;

    public CustomerRepository(AccountDbContext ctx)
    {
        _ctx = ctx;
    }
    
    public Task<AccountCustomer?> FindCustomer(Guid customerId, CancellationToken cancellationToken = default)
    {
        var customer = _ctx.Customers
            .Where(c => c.CustomerId.Equals(customerId))
            .FirstOrDefaultAsync(cancellationToken);

        return customer;
    }

    public async Task InsertCustomer(AccountCustomer accountCustomer, CancellationToken cancellationToken = default)
    {
        _ctx.Customers.Add(accountCustomer);
        await _ctx.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateCustomer(AccountCustomer accountCustomer, CancellationToken cancellationToken = default)
    {
        _ctx.Entry(accountCustomer).CurrentValues.SetValues(accountCustomer);
        await _ctx.SaveChangesAsync(cancellationToken);
    }

    public void DeleteCustomer(AccountCustomer customer)
    {
        _ctx.Customers.Remove(customer);
    }
}