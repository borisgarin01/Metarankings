namespace Data.Migrations;

[Migration(53, "Add games genres order column migration")]
public sealed class AddGamesGenresOrderColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE GamesGenres DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE GamesGenres ADD Position SMALLINT NULL;");
    }
}
