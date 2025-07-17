namespace Data.Migrations.Games.CreateTables.Comments;

[Migration(25, "Create games reviews table migration")]
public sealed class CreateGamesReviewsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesReviews");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesReviews
(
Id bigint not null primary key identity(1,1),
GameId bigint not null,
ApplicationUserId bigint not null,
UNIQUE(GameId, ApplicationUserId),
ReviewText NVARCHAR(MAX) NOT NULL,
FOREIGN KEY(GameId) 
    REFERENCES Games(Id) 
    ON DELETE CASCADE,
FOREIGN KEY(ApplicationUserId)
    REFERENCES ApplicationUsers(Id)
    ON DELETE CASCADE
);");
    }
}
