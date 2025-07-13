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
    UserName nvarchar(256) NOT NULL,
    NormalizedUserName nvarchar(256) NOT NULL,
    Email nvarchar(256) NULL,
    NormalizedEmail nvarchar(256) NULL,
    EmailConfirmed bit NOT NULL default 0,
    PasswordHash nvarchar(255) NULL,
    PhoneNumber nvarchar(50) NULL,
    PhoneNumberConfirmed bit NOT NULL default 0,
    TwoFactorEnabled bit NOT NULL default 0
)");
    }
}
