namespace Data.Migrations;

[Migration(43, "Create movies critics reviews table migration")]
public sealed class CreateMoviesCriticsReviewsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesCriticsReviews;");
    }

    public override void Up()
    {
        Execute.Sql(@"
CREATE TABLE MoviesCriticsReviews
(Id bigserial not null primary key,
MovieId bigint not null,
UserId bigint not null,
TextContent text not null,
Score float not null,
Date date not null,
CHECK (Score >= 0 and Score <= 10),
UNIQUE (MovieId, UserId),
FOREIGN KEY(MovieId) 
    REFERENCES Movies(Id)
    ON DELETE CASCADE,
FOREIGN KEY(UserId)
    REFERENCES ApplicationUsers(Id)
    ON DELETE CASCADE);");
    }
}
