namespace Data.Migrations;

[Migration(33, "Create viewers movies reviews shifts table")]
public sealed class CreateViewersMoviesReviewsShiftsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE ViewersMoviesReviewsShifts;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE ViewersMoviesReviewsShifts
(Id bigint not null primary key identity(1,1), 
ViewerMovieReviewId bigint not null,
ShifterId bigint not null,
Direction bit not null default 1, -- 1 - up, 0 - down
UNIQUE(ViewerMovieReviewId, ShifterId),
FOREIGN KEY (ViewerMovieReviewId) 
REFERENCES ViewersMoviesReviews(Id)
ON DELETE CASCADE,
FOREIGN KEY (ShifterId)
REFERENCES ApplicationUsers(Id)
ON DELETE NO ACTION);");
    }
}
