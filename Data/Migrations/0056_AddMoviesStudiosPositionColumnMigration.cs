namespace Data.Migrations;

[Migration(56, "Add movies studios order column migration")]
public sealed class AddMoviesStudiosOrderColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE MoviesMoviesStudios DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE MoviesMoviesStudios ADD Position SMALLINT NULL;");
    }
}
