namespace Data.Migrations;

[Migration(54, "Add games screenshots order column migration")]
public sealed class AddGamesScreenshotsOrderColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE GamesScreenshots DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE GamesScreenshots ADD Position SMALLINT NULL;");
    }
}
