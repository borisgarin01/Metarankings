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
    LoginProvider nvarchar(127) not null,
    Name nvarchar(127) not null,
    Value nvarchar(511) not null
);");
    }
}
