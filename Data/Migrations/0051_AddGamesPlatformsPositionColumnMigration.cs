namespace Data.Migrations;

[Migration(51, "Add games platforms order column migration")]
public sealed class AddGamesPlatformsPositionColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE GamesPlatforms DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE GamesPlatforms ADD Position SMALLINT NULL;");
    }
}
