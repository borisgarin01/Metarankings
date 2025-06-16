using FluentMigrator;

namespace Data.Migrations.Games.CreateTables;

[Migration(3, "Add localizations table migration")]
public sealed class CreateLocalizationsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Localizations;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Localizations 
(Id bigint not null primary key identity(1,1),
Name varchar(127) not null unique);");
    }
}
