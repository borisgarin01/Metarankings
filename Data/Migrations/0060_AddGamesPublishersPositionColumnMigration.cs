namespace Data.Migrations;

[Migration(60, "Add gamse publishers position column migration")]
public sealed class AddGamesPublishersPositionColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE GamesPublishers DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE GamesPublishers ADD Position SMALLINT NULL;");
    }
}
