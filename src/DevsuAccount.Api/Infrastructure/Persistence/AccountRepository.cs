using DevsuAccount.Api.Features.Account;
using DevsuAccount.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DevsuAccount.Api.Infrastructure.Persistence;

public class AccountRepository : IAccountRepository
{
    private readonly AccountDbContext _ctx;

    public AccountRepository(AccountDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<Account?> FindAccount(string accountNumber, CancellationToken cancellationToken = default)
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

    public void UpdateAccount(Account account)
    {
        _ctx.Accounts.Update(account);
    }

    public void DeleteAccount(Account customer)
    {
        _ctx.Accounts.Remove(customer);
    }

    public void AddTransaction(AccountTransaction newTransaction)
    {
        _ctx.Transactions.Add(newTransaction);
    }
    
    public Task<Account?> FindAccount(Guid accountTransactionId, CancellationToken cancellationToken = default)
    {
        var account = _ctx.Accounts
            .AsSplitQuery()
            .Include(a => a.Transactions)
            .Where(a => a.Transactions.Any(t => t.TransactionId == accountTransactionId))
            .FirstOrDefaultAsync(cancellationToken);
        
        return account;
    }

    public Task<List<Account>> GetActiveAccounts(Guid customerId, CancellationToken cancellationToken = default)
    {
        var accounts = _ctx.Accounts
            .Where(a => a.CustomerId == customerId && a.State == true)
            .ToListAsync(cancellationToken);
    
        return accounts ;
    }

    public void DeleteAccountTransaction(AccountTransaction accountTransaction)
    {
        _ctx.Transactions.Remove(accountTransaction);
    }

    public async Task<GetAccountStateReportResult> GetAccountState(Guid requestCustomerId, DateTime initialDate,
        DateTime finalDate, int page, int size,
        CancellationToken cancellationToken = default)
    {
        //TODO: improve with raw query and indexes
        var totalTransactions = await _ctx.Transactions
            .AsSplitQuery()
            .Include(t => t.Account)
            .Where(t => t.Account.CustomerId == requestCustomerId && t.DateCreation >= initialDate && t.DateCreation <= finalDate)
            .Select(t => t.TransactionId)
            .CountAsync(cancellationToken);
        
        var transactions = await _ctx.Transactions
            .AsSplitQuery()
            .Include(t => t.Account)
            .Where(t => t.Account.CustomerId == requestCustomerId && t.DateCreation >= initialDate && t.DateCreation <= finalDate)
            .OrderBy(t => t.DateCreation)
            .Take(size)
            .Skip(page*size)
            .Select(t => new GetAccountStateReportDetailsResult
            {
                Fecha = t.DateCreation,
                NumeroCuenta = t.Account.AccountNumber,
                Tipo = t.Account.AccountType.Value,
                SaldoInicial = t.Account.OpeningBalance,
                Estado = t.Account.State,
                Movimiento = t.TransactionValue,
                SaldoDisponible = t.Balance
            })
            .ToListAsync(cancellationToken);
        
        GetAccountStateReportResult result = new()
        {
            Data = transactions,
            Pagina = page,
            TamanoBatch = size,
            NumeroTotalRegistros = totalTransactions
        };
        return result;
    }

    public Task<int> SaveEntities(CancellationToken cancellationToken = default)
    {
       return _ctx.SaveChangesAsync(cancellationToken);
    }
}