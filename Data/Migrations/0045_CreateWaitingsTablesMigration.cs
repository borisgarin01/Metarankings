namespace Data.Migrations;

[Migration(45, "Create games and movies waitings tables migration")]
public sealed class CreateWaitingsTablesMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesWaitings;");
        Execute.Sql("DROP TABLE MoviesWaitings;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesWaitings
(
Id BIGSERIAL NOT NULL PRIMARY KEY,
GameId BIGINT NOT NULL,
UserId BIGINT NOT NULL,
IsWaiting BOOLEAN NOT NULL, -- true - жду, false - не жду
UNIQUE(GameId, UserId),
FOREIGN KEY(GameId)
REFERENCES Games(Id)
ON DELETE CASCADE,
FOREIGN KEY(UserId)
REFERENCES ApplicationUsers(Id)
ON DELETE CASCADE);");

        Execute.Sql("CREATE INDEX IX_GamesWaitings_GameId ON GamesWaitings(GameId);");

        Execute.Sql(@"CREATE TABLE MoviesWaitings
(
Id BIGSERIAL NOT NULL PRIMARY KEY,
MovieId BIGINT NOT NULL,
UserId BIGINT NOT NULL,
IsWaiting BOOLEAN NOT NULL, -- true - жду, false - не жду
UNIQUE(MovieId, UserId),
FOREIGN KEY(MovieId)
REFERENCES Movies(Id)
ON DELETE CASCADE,
FOREIGN KEY(UserId)
REFERENCES ApplicationUsers(Id)
ON DELETE CASCADE);");

        Execute.Sql("CREATE INDEX IX_MoviesWaitings_MovieId ON MoviesWaitings(MovieId);");
    }
}
