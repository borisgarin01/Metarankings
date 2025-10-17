namespace Data.Migrations;

[Migration(32, "Create games players reviews shifts table")]
public sealed class CreateGamesPlayersReviewsShiftsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesPlayersReviewsShifts;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesPlayersReviewsShifts
(Id bigserial not null primary key, 
GamePlayerReviewId bigint not null,
ShifterId bigint not null,
Direction bit not null, -- 1 - up, 0 - down
UNIQUE(GamePlayerReviewId, ShifterId),
FOREIGN KEY (GamePlayerReviewId) 
REFERENCES GamesPlayersReviews(Id)
ON DELETE CASCADE,
FOREIGN KEY (ShifterId)
REFERENCES ApplicationUsers(Id)
ON DELETE NO ACTION);");
    }
}
