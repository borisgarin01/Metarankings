namespace Data.Migrations;

[Migration(9, "Add games genres table migration")]
public sealed class CreateGamesGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesGenres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesGenres(Id bigint primary key not null identity(1,1),
GameId bigint not null, 
GenreId bigint not null,
CONSTRAINT UNIQUE_GameId_GenreId UNIQUE(GameId, GenreId),
FOREIGN KEY(GameId) REFERENCES Games(Id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY(GenreId) REFERENCES Genres(Id) ON UPDATE CASCADE ON DELETE CASCADE
);");
    }
}
