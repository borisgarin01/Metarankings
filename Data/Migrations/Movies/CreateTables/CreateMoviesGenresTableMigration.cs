using FluentMigrator;

namespace Data.Migrations.Movies.CreateTables;

[Migration(11, "Create movies genres table migration")]
public sealed class CreateMoviesGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesGenres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesGenres (id bigserial not null primary key,
name varchar(511) not null unique);");
    }
}
