namespace Devsu.Shared.Primitives;

public interface IRepository<T> where T : IAggregateRoot
{
    Task<int> SaveEntities(CancellationToken cancellationToken = default);
}