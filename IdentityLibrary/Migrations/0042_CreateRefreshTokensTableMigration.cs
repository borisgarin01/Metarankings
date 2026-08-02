namespace IdentityLibrary.Migrations;

[Migration(42, "Create refresh tokens table migration")]
public sealed class CreateRefreshTokensTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP CONSTRAINT FK_RefreshTokens_Users;
DROP TABLE RefreshTokens;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE RefreshTokens(
Id bigserial not null primary key, 
UserId bigint not null, 
Value varchar(1023) not null,
IsRevoked BOOLEAN NOT NULL DEFAULT FALSE,
CreatedAt TIMESTAMP NOT NULL,
CONSTRAINT FK_RefreshTokens_Users 
    FOREIGN KEY (UserId) 
        REFERENCES ApplicationUsers(Id)
            ON DELETE CASCADE);");
    }
}
