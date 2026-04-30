namespace Data.Migrations;

[Migration(41, "Create movies collections items table migration")]
public sealed class CreateMoviesCollectionsItemsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesCollectionsItems;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesCollectionsItems
(
Id BIGSERIAL not null primary key,
MovieCollectionId BIGINT NOT NULL, 
MovieId BIGINT NOT NULL, 
UNIQUE(MovieCollectionId, MovieId),
FOREIGN KEY(MovieCollectionId) 
REFERENCES MoviesCollections(Id) 
ON DELETE CASCADE,
FOREIGN KEY(MovieId) 
REFERENCES Movies(Id) 
ON DELETE CASCADE);");
    }
}
