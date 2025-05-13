using FluentMigrator;

namespace Data.Migrations;

[Migration(3, "Add localizations table migration")]
public sealed class AddLocalizationsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Localizations;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Localizations 
(Id bigserial not null primary key,
Href varchar(511) not null unique, 
Name varchar(127) not null unique);");
    }
}
