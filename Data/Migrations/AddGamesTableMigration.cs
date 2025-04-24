using FluentMigrator;

namespace Data.Migrations;

[Migration(1, "Add games table migration")]
public sealed class AddGamesTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE Games;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Games
(Id bigserial not null primary key,
Href varchar(1023) not null unique,
Name varchar(1023) not null unique,
Image varchar(1023) null unique,
LocalizationId bigint null,
PublisherId bigint not null,
ReleaseDate date null,
Description text,
Trailer varchar(511) null);");
    }
}
