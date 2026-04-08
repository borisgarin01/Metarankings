namespace Data.Migrations;

[Migration(35, "Create games publishers table")]
public sealed class CreateGamesPublishersTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesPublishers;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesPublishers(GameId BIGINT NOT NULL, PublisherId BIGINT NOT NULL, 
FOREIGN KEY(GameId) 
REFERENCES Games(Id) 
ON DELETE CASCADE
ON UPDATE CASCADE);");
    }
}
