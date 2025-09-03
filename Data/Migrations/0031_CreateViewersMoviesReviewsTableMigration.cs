namespace Data.Migrations;

[Migration(31, "Create viewers movies reviews table")]
public sealed class CreateViewersMoviesReviewsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE ViewersMoviesReviews;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE ViewersMoviesReviews
(ViewerId bigint not null, 
MovieId bigint not null, 
Score float not null, 
TextContent nvarchar(max), 
Date datetime not null,
UNIQUE(ViewerId, MovieId));");
    }
}
