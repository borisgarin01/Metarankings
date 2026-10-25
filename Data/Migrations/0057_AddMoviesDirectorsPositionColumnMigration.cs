namespace Data.Migrations;

[Migration(57, "Add movies directors order column migration")]
public sealed class AddMoviesDirectorsOrderColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE MoviesMoviesDirectors DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE MoviesMoviesDirectors ADD Position SMALLINT NULL;");
    }
}
