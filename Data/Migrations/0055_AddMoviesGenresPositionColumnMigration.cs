namespace Data.Migrations;

[Migration(55, "Add movies genres order column migration")]
public sealed class AddMoviesGenresOrderColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE MoviesMoviesGenres DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE MoviesMoviesGenres ADD Position SMALLINT NULL;");
    }
}
