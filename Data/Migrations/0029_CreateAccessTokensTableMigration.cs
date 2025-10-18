namespace Data.Migrations;

[Migration(29, "Create access tokens table migration")]
public sealed class CreateAccessTokensTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE AccessTokens;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE AccessTokens
(
    UserId bigint not null, 
    LoginProvider varchar(127) not null,
    Name varchar(127) not null,
    Value varchar(511) not null
);");
    }
}
