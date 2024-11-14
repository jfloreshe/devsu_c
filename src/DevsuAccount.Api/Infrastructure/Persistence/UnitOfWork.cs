using System.Data;
using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevsuAccount.Api.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AccountDbContext _ctx;

    public UnitOfWork(AccountDbContext ctx)
    {
        _ctx = ctx;
    }

    private IAccountRepository _accountRepository;
    private ICustomerRepository _customerRepository;
    public IAccountRepository AccountRepository => _accountRepository ??= new AccountRepository(_ctx);
    public ICustomerRepository CustomerRepository => _customerRepository ??= new CustomerRepository(_ctx);
    
    public void Dispose() => _ctx.Dispose();
    
    public Task<int> SaveChanges()
    {
        return _ctx.SaveChangesAsync();
    }

    public async Task<T> ExecuteTransaction<T>(IUnitOfWork.DbTransaction<T> action)
    {
        var strategy = _ctx.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await _ctx.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            try
            {
                T response = await action();

                await transaction.CommitAsync();

                return response;
            }
            catch
            {
                await transaction.RollbackAsync();

                throw;
            }
        });
    }
}