using Dapper;
using IdentityLibrary.DTOs;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace IdentityLibrary.Repositories;

public sealed class RoleStore : IRoleStore<ApplicationRole>
{
    private readonly string _connectionString;

    public RoleStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            role.Id = await connection.QuerySingleAsync<int>($@"INSERT INTO [ApplicationRoles] ([Name])
                    VALUES (@Name)
                    RETURNING Id;", role);
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteAsync($"DELETE FROM [ApplicationRoles] WHERE [Id] = @Id", role);
        }

        return IdentityResult.Success;
    }

    public void Dispose()
    {
    }

    public async Task<ApplicationRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<ApplicationRole>($@"SELECT * FROM [ApplicationRoles]
                    WHERE [Id] = @roleId", new { roleId });
        }
    }

    public async Task<ApplicationRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<ApplicationRole>($@"SELECT * FROM [ApplicationRoles]
                    WHERE [NormalizedName] = @normalizedRoleName", new { normalizedRoleName });
        }
    }

    public Task<string?> GetNormalizedRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.NormalizedName);
    }

    public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id.ToString());
    }

    public Task<string?> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetNormalizedRoleNameAsync(ApplicationRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        role.NormalizedName = normalizedName;
        return Task.FromResult(0);
    }

    public Task SetRoleNameAsync(ApplicationRole role, string? roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;
        return Task.FromResult(0);
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteAsync($@"UPDATE [ApplicationRoles] SET
                    [Name] = @Name
                    WHERE [Id] = @Id", role);
        }

        return IdentityResult.Success;
    }
}
