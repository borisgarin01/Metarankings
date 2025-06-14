using Dapper;
using IdentityLibrary.DTOs;
using Microsoft.AspNetCore.Identity;
using Npgsql;

namespace IdentityLibrary.Repositories;

public sealed class UsersStore : IUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IUserPhoneNumberStore<ApplicationUser>,
        IUserTwoFactorStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
{
    private readonly string _connectionString;

    public UsersStore(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var roleId = await connection.ExecuteScalarAsync<int?>($"SELECT Id FROM ApplicationRoles WHERE Name = @roleName;", new { roleName });
            if (!roleId.HasValue)
                roleId = await connection.ExecuteAsync($"INSERT INTO ApplicationRoles(Name) VALUES(@roleName)",
                    new { roleName });

            await connection.ExecuteAsync(@"INSERT INTO ApplicationUsersRoles(UserId, RoleId) 
VALUES(@userId, @roleId)
ON CONFLICT DO NOTHING;",
                new { userId = user.Id, roleId });
        }
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            user.Id = await connection.QuerySingleAsync<int>($@"INSERT INTO ApplicationUsers (UserName, Email,
                    EmailConfirmed, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled)
                    VALUES (@UserName, @Email, CAST @EmailConfirmed as bit, @PasswordHash,
                    @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled) 
RETURNING Id", user);
        }

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.ExecuteAsync($"DELETE FROM ApplicationUsers WHERE Id = @Id", user.Id);
        }

        return IdentityResult.Success;
    }

    public void Dispose()
    {
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ApplicationUsers
                    WHERE Email = @email", new { email });
        }
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ApplicationUsers
                    WHERE Id = @userId", new { userId });
        }
    }

    public async Task<ApplicationUser?> FindByNameAsync(string userName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            return await connection.QuerySingleOrDefaultAsync<ApplicationUser>($@"SELECT * FROM ApplicationUsers
                    WHERE UserName = @userName", new { userName });
        }
    }

    public Task<string?> GetEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email);
    }

    public Task<bool> GetEmailConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.EmailConfirmed);
    }

    public Task<string?> GetNormalizedEmailAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Email.ToUpperInvariant());
    }

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName.ToUpperInvariant());
    }

    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public Task<string?> GetPhoneNumberAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumber);
    }

    public Task<bool> GetPhoneNumberConfirmedAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PhoneNumberConfirmed);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.OpenAsync(cancellationToken);
            var queryResults = await connection.QueryAsync<string>(@"SELECT r.Name FROM ApplicationRoles r 
INNER JOIN ApplicationUsersRoles ur ON ur.RoleId = r.Id 
WHERE ur.UserId = @userId", new { userId = user.Id });

            return queryResults.ToList();
        }
    }

    public Task<bool> GetTwoFactorEnabledAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.TwoFactorEnabled);
    }

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.ToString());
    }

    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName.ToString());
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var queryResults = await connection.QueryAsync<ApplicationUser>(@"SELECT u.* FROM ApplicationUsers u 
INNER JOIN ApplicationUsersRoles ur 
ON ur.UserId = u.Id 
INNER JOIN ApplicationRoles r ON r.Id = ur.RoleId 
WHERE r.NormalizedName = @normalizedName",
                new { normalizedName = roleName.ToUpper() });

            return queryResults.ToList();
        }
    }

    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash != null);
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var roleId = await connection.ExecuteScalarAsync<int?>(@"SELECT Id 
FROM ApplicationRoles 
WHERE NormalizedName = @normalizedName", new { normalizedName = roleName.ToUpper() });
            if (roleId is null ^ roleId == 0)
                return false;
            var matchingRoles = await connection.ExecuteScalarAsync<int>($@"SELECT COUNT(*) 
FROM ApplicationUsersRoles 
WHERE UserId = @userId 
    AND RoleId = @roleId",
                new { userId = user.Id, roleId });

            return matchingRoles > 0;
        }
    }

    public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            var roleId = await connection.ExecuteScalarAsync<int?>(@"SELECT Id 
FROM ApplicationRoles 
WHERE NormalizedName = @normalizedName", new { normalizedName = roleName.ToUpper() });
            if (!roleId.HasValue)
                await connection.ExecuteAsync(@"DELETE FROM ApplicationUsersRoles 
WHERE UserId = @userId 
    AND RoleId = @roleId", new { userId = user.Id, roleId });
        }
    }

    public Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
    {
        user.Email = email;
        return Task.FromResult(0);
    }

    public Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.EmailConfirmed = confirmed;
        return Task.FromResult(0);
    }

    public Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        user.NormalizedEmail = normalizedEmail;
        return Task.FromResult(0);
    }

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.FromResult(0);
    }

    public Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        user.PasswordHash = passwordHash;
        return Task.FromResult(0);
    }

    public Task SetPhoneNumberAsync(ApplicationUser user, string? phoneNumber, CancellationToken cancellationToken)
    {
        user.PhoneNumber = phoneNumber;
        return Task.FromResult(0);
    }

    public Task SetPhoneNumberConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        user.PhoneNumberConfirmed = confirmed;
        return Task.FromResult(0);
    }

    public Task SetTwoFactorEnabledAsync(ApplicationUser user, bool enabled, CancellationToken cancellationToken)
    {
        user.TwoFactorEnabled = enabled;
        return Task.FromResult(0);
    }

    public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.FromResult(0);
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using (var connection = new NpgsqlConnection(_connectionString))
        {
            await connection.ExecuteAsync($@"UPDATE ApplicationUsers SET
                    UserName = @UserName,
                    NormalizedUserName = @NormalizedUserName,
                    Email = @Email,
                    NormalizedEmail = @NormalizedEmail,
                    EmailConfirmed = @EmailConfirmed,
                    PasswordHash = @PasswordHash,
                    PhoneNumber = @PhoneNumber,
                    PhoneNumberConfirmed = @PhoneNumberConfirmed,
                    TwoFactorEnabled = @TwoFactorEnabled
                    WHERE Id = @Id", user);
        }

        return IdentityResult.Success;
    }
}
