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
(
Id bigint not null primary key identity(1,1),
ViewerId bigint not null, 
MovieId bigint not null, 
Score float not null, 
TextContent nvarchar(max), 
Date datetime not null,
UNIQUE(ViewerId, MovieId));");
    }
}
