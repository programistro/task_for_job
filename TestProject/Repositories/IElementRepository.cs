using TestProject.Entities;

namespace TestProject.Repositories;

public interface IElementRepository
{
    Task<long> AddAsync(string content, string attribute, CancellationToken cancellationToken = default);

    Task<Element?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Element>> GetAllAsync(int limit, int offset, CancellationToken cancellationToken = default);

    Task<int> CountAsync(CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default);
}
