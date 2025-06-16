using FluentMigrator;

namespace IdentityLibrary.Migrations;

[Migration(21, "Create application roles table migration")]
public sealed class CreateApplicationRolesTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE ApplicationRoles;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE ApplicationRoles
(	
	Id BIGSERIAL NOT NULL PRIMARY KEY,
    Name VARCHAR(256) NOT NULL,
    NormalizedName VARCHAR(256) NOT NULL
);");
    }
}
