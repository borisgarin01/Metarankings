using IdentityLibrary.DTOs;

namespace IdentityLibrary.Repositories;

public sealed class UsersStore : IUserStore<ApplicationUser>, IUserEmailStore<ApplicationUser>, IQueryableUserStore<ApplicationUser>, IUserPasswordStore<ApplicationUser>, IUserRoleStore<ApplicationUser>
{
    private readonly string _connectionString;

    public UsersStore(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("MetarankingsConnection");
    }

    public IQueryable<ApplicationUser> Users
    {
        get
        {
            using var connection = new SqlConnection(_connectionString);
            var users = connection.Query<ApplicationUser>(@"SELECT Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled 
FROM ApplicationUsers;");

            return users.AsQueryable();
        }
    }

    public async Task AddToRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"DECLARE @RoleId int; 
SELECT @RoleId=(SELECT Id FROM ApplicationRoles where NormalizedName=UPPER(@roleName));
IF @RoleId is not null
BEGIN 
    INSERT INTO ApplicationUsersRoles (UserId, RoleId) VALUES(@UserId, @RoleId);
END;", new { roleName, UserId = user.Id });
    }

    public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"INSERT INTO ApplicationUsers (UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled) 
VALUES
(@UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled);", user);

        return IdentityResult.Success;
    }

    public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"DELETE FROM ApplicationUsers WHERE Id=@Id;", new { user.Id });

        return IdentityResult.Success;
    }

    public void Dispose()
    {
    }

    public async Task<ApplicationUser?> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        var applicationUser = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(@"SELECT Id, UserName, NormalizedUserName, Email, EmailConfirmed, NormalizedEmail, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled
FROM ApplicationUsers 
WHERE NormalizedEmail = @normalizedEmail;", new { normalizedEmail });

        return applicationUser;
    }

    public async Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        var applicationUser = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(@"SELECT Id, UserName, NormalizedUserName, Email, EmailConfirmed, NormalizedEmail, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled
FROM ApplicationUsers 
WHERE Id = @Id;", new { Id = userId });

        return applicationUser;
    }

    public async Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        var applicationUser = await connection.QueryFirstOrDefaultAsync<ApplicationUser>(@"SELECT Id, UserName, NormalizedUserName, Email, EmailConfirmed, NormalizedEmail, PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled
FROM ApplicationUsers 
WHERE NormalizedUserName = @normalizedUserName;", new { normalizedUserName });

        return applicationUser;
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
        return Task.FromResult(user.NormalizedEmail);
    }

    public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task<string?> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.PasswordHash);
    }

    public async Task<IList<string>> GetRolesAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        return (await connection.QueryAsync<string>(@"SELECT Name 
    FROM ApplicationRoles 
    WHERE Id in 
        (SELECT RoleId 
            FROM ApplicationUsersRoles 
            WHERE UserId = @Id);", new { user.Id })).ToList();
    }

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id.ToString());
    }

    public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        var applicationUsers = await connection.QueryAsync<ApplicationUser>(
            @"SELECT Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, 
                    PasswordHash, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled
                FROM ApplicationUsers
                    WHERE Id IN
                    (SELECT UserId
                        FROM ApplicationUsersRoles
                            WHERE RoleId IN
                            (SELECT RoleId 
                                FROM ApplicationRoles
                                    WHERE NormalizedName = UPPER(@roleName);", new { roleName });

        return applicationUsers.ToList();
    }

    public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(!string.IsNullOrWhiteSpace(user.PasswordHash));
    }

    public async Task<bool> IsInRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        var givenApplicationUserGivenApplicationRolesCount = await connection.QueryFirstAsync<long>(@"SELECT COUNT(*)
FROM ApplicationUsersRoles
WHERE UserId = @UserId
    AND RoleId =
        (SELECT Id 
            FROM ApplicationRoles 
                WHERE NormalizedName = UPPER(@roleName));", new { UserId = user.Id, RoleName = roleName });

        return givenApplicationUserGivenApplicationRolesCount > 0;
    }

    public async Task RemoveFromRoleAsync(ApplicationUser user, string roleName, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"DELETE FROM ApplicationUsersRoles 
WHERE UserId=@UserId 
    AND RoleId = (SELECT Id 
        FROM ApplicationRoles
            WHERE NormalizedName = UPPER (@roleName));", new { roleName });
    }

    public async Task SetEmailAsync(ApplicationUser user, string? email, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"UPDATE ApplicationUsers 
SET Email=@email 
WHERE Id=@Id;", new { email, user.Id });
    }

    public async Task SetEmailConfirmedAsync(ApplicationUser user, bool confirmed, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        bool emailConfirmed = await connection.QueryFirstAsync<bool>(@"UPDATE ApplicationUsers 
SET EmailConfirmed=@confirmed 
OUTPUT inserted.EmailConfirmed
WHERE Id=@Id;", new { confirmed, user.Id });
        user.EmailConfirmed = emailConfirmed;
    }

    public async Task SetNormalizedEmailAsync(ApplicationUser user, string? normalizedEmail, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"UPDATE ApplicationUsers 
SET NormalizedEmail=@normalizedEmail 
WHERE Id=@Id;", new { normalizedEmail, user.Id });
    }

    public async Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedUsername, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"UPDATE ApplicationUsers 
SET NormalizedUserName=@normalizedUsername 
WHERE Id=@Id;", new { normalizedUsername, user.Id });
    }

    public async Task SetPasswordHashAsync(ApplicationUser user, string? passwordHash, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"UPDATE ApplicationUsers 
SET PasswordHash = @passwordHash 
WHERE Id=@Id;", new { passwordHash, user.Id });
    }

    public async Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"UPDATE ApplicationUsers 
SET UserName=@userName 
WHERE Id=@Id;", new { userName, user.Id });
    }

    public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        using var connection = new SqlConnection(_connectionString);
        await connection.ExecuteAsync(@"UPDATE ApplicationUsers 
SET 
UserName=@UserName, 
NormalizedUserName=@NormalizedUserName, 
Email=@Email,
NormalizedEmail=@NormalizedEmail,
EmailConfirmed=@EmailConfirmed,
PasswordHash=@PasswordHash,
PhoneNumber=@PhoneNumber,
PhoneNumberConfirmed=@PhoneNumberConfirmed,
TwoFactorEnabled=@TwoFactorEnabled WHERE Id=@Id;", user);

        return IdentityResult.Success;
    }
}