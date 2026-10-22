namespace Data.Migrations;

[Migration(59, "Add gamse tags order column migration")]
public sealed class AddGamesTagsOrderColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE GamesTags DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE GamesTags ADD Position SMALLINT NULL;");
    }
}
