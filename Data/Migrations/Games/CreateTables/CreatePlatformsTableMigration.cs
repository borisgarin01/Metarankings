using FluentMigrator;

namespace Data.Migrations.Games.CreateTables;

[Migration(4, "Add platforms table migration")]
public sealed class CreatePlatformsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Platforms;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Platforms 
(Id bigserial not null primary key,
Href varchar(511) not null unique,
Name varchar(127) not null unique);");
    }
}
