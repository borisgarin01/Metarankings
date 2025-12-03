namespace Data.Migrations;

[Migration(37, "Create game collection items table migration")]
public sealed class CreateGameCollectionItemsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesCollectionsItems;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesCollectionsItems
(
Id BIGSERIAL not null primary key,
GameCollectionId BIGINT NOT NULL, 
GameId BIGINT NOT NULL, 
FOREIGN KEY(GameCollectionId) 
REFERENCES GamesCollections(Id) 
ON DELETE CASCADE,
FOREIGN KEY(GameId) 
REFERENCES Games(Id) 
ON DELETE CASCADE);");
    }
}
