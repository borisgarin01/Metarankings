namespace Data.Migrations;

[Migration(58,"Add movies tags order column migration")]
public sealed class AddMoviesTagsOrderColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE MoviesTags DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE MoviesTags ADD Position SMALLINT NULL;");
    }
}
