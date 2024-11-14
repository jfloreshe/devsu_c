namespace DevsuAccount.Api.Models;

public interface IUnitOfWork : IDisposable
{
    public delegate Task<T> DbTransaction<T>();
    IAccountRepository AccountRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    Task<int> SaveChanges();
    Task<T> ExecuteTransaction<T>(DbTransaction<T> action);
}