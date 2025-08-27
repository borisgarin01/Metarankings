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

    public async Task SetNormalizedRoleNameAsync(ApplicationRole role, string? normalizedName, CancellationToken cancellationToken)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.ExecuteAsync(@"UPDATE ApplicationRoles 
SET NormalizedName=@normalizedName
WHERE Id=@Id;", new { normalizedName, role.Id });
            role.NormalizedName = normalizedName;
        }
    }

    public Task<string> GetRoleIdAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id.ToString());
    }

    public Task<string?> GetRoleNameAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public async Task SetRoleNameAsync(ApplicationRole role, string? roleName, CancellationToken cancellationToken)
    {
        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.ExecuteAsync(@"UPDATE ApplicationRoles 
SET Name=@roleName
WHERE Id=@Id;", new { roleName, role.Id });
            role.Name = roleName;
        }
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationRole role, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new SqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            await connection.ExecuteAsync($@"UPDATE [ApplicationRoles] SET
                    [Name] = @Name
                    WHERE str(Id) = @Id", role.Id);
        }

        return IdentityResult.Success;
    }
}
