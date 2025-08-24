namespace Data.Migrations;

[Migration(28, "Create games players reviews table")]
public sealed class CreateGamesPlayersReviewsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesPlayersReviews;");
    }

    public override void Up()
    {
        Execute.Sql(@"
CREATE TABLE GamesPlayersReviews
(Id bigint not null primary key identity(1,1),
GameId bigint not null,
UserId bigint not null,
TextContent nvarchar(MAX) not null,
Score float not null,
Date date not null,
CHECK (Score >= 0 and Score <= 10),
UNIQUE (GameId, UserId),
FOREIGN KEY(GameId) 
    REFERENCES Games(Id)
    ON DELETE CASCADE,
FOREIGN KEY(UserId)
    REFERENCES ApplicationUsers(Id)
    ON DELETE CASCADE);");
    }
}
