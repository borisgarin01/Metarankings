namespace IdentityLibrary.Migrations;

[Migration(24, "Insert admin and user roles migration")]
public sealed class InsertAdminAndUserRolesMigration : Migration
{
    public override void Down()
    {
        var roles = new string[] { "Admin", "User" };

        Execute.Sql(@"DELETE FROM ApplicationRoles WHERE NormalizedName IN (@roles);".Replace("@roles", string.Join(",", roles.Select(r => $"'{r}'"))));
    }

    public override void Up()
    {
        var roles = new string[] { "Admin", "User" };

        foreach (var role in roles)
        {
            Execute.Sql(@"INSERT INTO ApplicationRoles(Name, NormalizedName)
VALUES(@Name, UPPER(@Name))".Replace("@Name", $"'{role}'"));  // Fixed: Added closing parenthesis and proper parameter
        }
    }
}