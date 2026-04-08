namespace IdentityLibrary.Migrations;

[Migration(38, "Create AspNetUserLogins table migration")]
public sealed class CreateAspNetUserLoginsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE AspNetUserLogins;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE AspNetUserLogins 
(
    LoginProvider varchar(255) not null,
    ProviderKey varchar(255) not null,
    ProviderDisplayName varchar(255) not null,
    UserId bigint not null,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY(UserId) 
        REFERENCES ApplicationUsers(Id) 
        ON DELETE CASCADE
);");
    }
}
