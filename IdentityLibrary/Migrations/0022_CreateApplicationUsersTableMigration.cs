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
	Id bigserial not null primary key,
    UserName varchar(256) NOT NULL,
    NormalizedUserName varchar(256) NOT NULL,
    Email varchar(256) NULL,
    NormalizedEmail varchar(256) NULL,
    EmailConfirmed boolean NOT NULL default FALSE,
    PasswordHash varchar(255) NULL,
    PhoneNumber varchar(50) NULL,
    PhoneNumberConfirmed boolean NOT NULL default FALSE,
    TwoFactorEnabled boolean NOT NULL default FALSE
)");
    }
}
