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
(Id bigint not null primary key identity(1,1), 
GamePlayerReviewId bigint not null,
ShifterId bigint not null,
Direction bit not null default 1, -- 1 - up, 0 - down
UNIQUE(GamePlayerReviewId, ShifterId),
FOREIGN KEY (GamePlayerReviewId) 
REFERENCES GamesPlayersReviews(Id)
ON DELETE CASCADE,
FOREIGN KEY (ShifterId)
REFERENCES ApplicationUsers(Id)
ON DELETE NO ACTION);");
    }
}
