using FluentMigrator;

namespace Data.Migrations.Games.CreateTables;

[Migration(10, "Add games screenshots table migration")]
public sealed class CreateGamesScreenshotsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE GamesScreenshots;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesScreenshots 
(Id bigserial not null primary key,
Url varchar(1023) not null unique,
GameId bigint not null,
FOREIGN KEY(GameId) 
REFERENCES Games(Id) 
ON DELETE CASCADE 
ON UPDATE CASCADE);");
    }
}
