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
	Id bigint not null primary key identity(1,1),
    UserName VARCHAR(256) NOT NULL,
    NormalizedUserName VARCHAR(256) NOT NULL,
    Email VARCHAR(256) NULL,
    NormalizedEmail VARCHAR(256) NULL,
    EmailConfirmed bit NOT NULL default 0,
    PasswordHash VARCHAR(255) NULL,
    PhoneNumber VARCHAR(50) NULL,
    PhoneNumberConfirmed bit NOT NULL default 0,
    TwoFactorEnabled bit NOT NULL default 0
)");
    }
}
