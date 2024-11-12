﻿using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevsuAccount.Api.Infrastructure.Persistence;

public class AccountRepository : IAccountRepository
{
    private readonly AccountDbContext _ctx;

    public AccountRepository(AccountDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<Account?> FindAccountAsync(string accountNumber, CancellationToken cancellationToken = default)
    {
        var account = _ctx.Accounts
            .AsSplitQuery()
            .Include(a => a.Transactions)
            .Where(c => c.AccountNumber == accountNumber)
            .FirstOrDefaultAsync(cancellationToken);
        
        return account;
    }

    public void AddAccount(Account newAccount)
    {
        _ctx.Accounts.Add(newAccount);
    }
    
    public Task<int> SaveEntities(CancellationToken cancellationToken = default)
    {
        return _ctx.SaveChangesAsync(cancellationToken);
    }
}