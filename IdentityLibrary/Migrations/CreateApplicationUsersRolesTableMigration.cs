namespace IdentityLibrary.Migrations;

[Migration(23, "Create application users roles table migration")]
public sealed class CreateApplicationUsersRolesTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE ApplicationUsersRoles;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE ApplicationUsersRoles
(
	UserId BIGINT NOT NULL,
	RoleId BIGINT NOT NULL,
    UNIQUE (UserId, RoleId),
    FOREIGN KEY (UserId) 
    REFERENCES ApplicationUsers(Id) 
    ON DELETE CASCADE,
    FOREIGN KEY (RoleId) 
    REFERENCES ApplicationRoles(Id) 
    ON DELETE CASCADE
);");
    }
}
