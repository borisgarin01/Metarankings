using IdentityLibrary.DTOs;

namespace IdentityLibrary.Repositories;

public sealed class RolesStore : IRoleStore<ApplicationRole>
{
    private readonly string _connectionString;

    public RolesStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MetarankingsConnection");
    }

    public async Task<IdentityResult> CreateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            role.Id = await connection.QuerySingleAsync<string>($@"INSERT INTO [ApplicationRoles] ([Name])
output str(id)
                    VALUES (@Name);", role);
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteAsync($"DELETE FROM [ApplicationRoles] WHERE str(Id) = @Id", role.Id);
        }

        return IdentityResult.Success;
    }

    public void Dispose()
    {
    }

    public async Task<ApplicationRole?> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<ApplicationRole>($@"SELECT * FROM [ApplicationRoles]
                    WHERE str(Id) = @roleId", new { roleId });
        }
    }

    public async Task<ApplicationRole?> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqlConnection(_connectionString))
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

    public Task SetNormalizedRoleNameAsync(ApplicationRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id.ToString());
    }

    public Task<string?> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetRoleNameAsync(ApplicationRole role, string? roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;
        return Task.CompletedTask;
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteAsync($@"UPDATE [ApplicationRoles] SET
                    [Name] = @Name,
                    [NormalizedName] = @NormalizedName,
                    ConcurrencyStamp=@ConcurrencyStamp
                    WHERE str(Id) = @Id", new
            {
                role.Name,
                role.NormalizedName,
                role.ConcurrencyStamp,
                role.Id
            });
        }

        return IdentityResult.Success;
    }
}
