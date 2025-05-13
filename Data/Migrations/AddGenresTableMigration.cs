using FluentMigrator;

namespace Data.Migrations;

[Migration(2, "Add games table migration")]
public sealed class AddGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE Genres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Genres (Id bigserial not null primary key,
Name varchar(255) not null unique,
Url varchar(1023) not null unique);");
    }
}
