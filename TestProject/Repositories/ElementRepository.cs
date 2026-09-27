using Dapper;
using Npgsql;
using TestProject.Entities;

namespace TestProject.Repositories;

public class ElementRepository : IElementRepository
{
    private const string SelectColumns = "id AS \"Id\", content AS \"Content\", \"attribute\" AS \"Attribute\"";

    private readonly NpgsqlDataSource _dataSource;

    public ElementRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
    }

    public async Task<long> AddAsync(string content, string attribute, CancellationToken cancellationToken = default)
    {
        const string sql = "INSERT INTO elements (content, \"attribute\") VALUES (@Content, @Attribute) RETURNING id;";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleAsync<long>(new CommandDefinition(sql, new { Content = content, Attribute = attribute }, cancellationToken: cancellationToken));
    }

    public async Task<Element?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = $"SELECT {SelectColumns} FROM elements WHERE id = @Id;";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<Element>(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
    }

    public async Task<IReadOnlyList<Element>> GetAllAsync(int limit, int offset, CancellationToken cancellationToken = default)
    {
        const string sql = $"SELECT {SelectColumns} FROM elements ORDER BY id LIMIT @Limit OFFSET @Offset;";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var rows = await connection.QueryAsync<Element>(new CommandDefinition(sql, new { Limit = limit, Offset = offset }, cancellationToken: cancellationToken));
        return rows.AsList();
    }

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        return await connection.ExecuteScalarAsync<int>(new CommandDefinition("SELECT COUNT(*) FROM elements;", cancellationToken: cancellationToken));
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken cancellationToken = default)
    {
        const string sql = "DELETE FROM elements WHERE id = @Id;";

        await using var connection = await _dataSource.OpenConnectionAsync(cancellationToken);
        var affected = await connection.ExecuteAsync(new CommandDefinition(sql, new { Id = id }, cancellationToken: cancellationToken));
        return affected > 0;
    }
}
