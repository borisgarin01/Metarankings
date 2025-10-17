namespace Data.Migrations;

[Migration(11, "Create movies genres table migration")]
public sealed class CreateMoviesGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesGenres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesGenres (Id bigserial not null primary key,
name varchar(511) not null unique);");
    }
}
