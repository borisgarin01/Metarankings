using FluentMigrator;

namespace IdentityLibrary.Migrations;

[Migration(22, "Create application users table migration")]
public sealed class CreateApplicationUsersTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE ApplicationUsers");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE ApplicationUsers
(
	Id BIGSERIAL NOT NULL PRIMARY KEY,
    UserName VARCHAR(256) NOT NULL,
    NormalizedUserName VARCHAR(256) NOT NULL,
    Email VARCHAR(256) NULL,
    NormalizedEmail VARCHAR(256) NULL,
    EmailConfirmed boolean NOT NULL,
    PasswordHash VARCHAR(255) NULL,
    PhoneNumber VARCHAR(50) NULL,
    PhoneNumberConfirmed boolean NOT NULL,
    TwoFactorEnabled boolean NOT NULL
)");
    }
}
