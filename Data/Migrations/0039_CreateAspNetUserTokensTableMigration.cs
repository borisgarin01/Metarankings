namespace Data.Migrations;

[Migration(39, "Create AspNetUserTokens table migration")]
public sealed class CreateAspNetUserTokensTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE AspNetUserTokens;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE AspNetUserTokens(UserId bigint not null, LoginProvider varchar(127) not null, Name varchar(255) not null, Value varchar(255) not null, 
UNIQUE(UserId, LoginProvider));");
    }
}
