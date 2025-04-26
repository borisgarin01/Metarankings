using FluentMigrator;

namespace Data.Migrations;

[Migration(1, "Add developers table migration")]
public sealed class AddDevelopersTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Developers;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Developers
(Id bigserial not null primary key,
Name varchar(511) not null unique,
Url varchar(1023) not null unique);");
    }
}
