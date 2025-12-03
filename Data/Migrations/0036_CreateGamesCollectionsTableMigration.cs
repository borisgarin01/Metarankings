namespace Data.Migrations;

[Migration(36, "Create games collections table migration")]
public sealed class CreateGamesCollectionsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE GamesCollections;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesCollections(Id BIGSERIAL NOT NULL PRIMARY KEY, 
Name VARCHAR(255) NOT NULL UNIQUE,
Description VARCHAR(MAX));");
    }
}
