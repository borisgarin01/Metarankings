using FluentMigrator;

namespace Data.Migrations.Movies.CreateTables;

[Migration(21, "Create movies movies genres table migration")]
public sealed class CreateMoviesMoviesGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesMoviesGenres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesMoviesGenres
(Id bigserial not null primary key,
MovieId bigint not null,
MovieGenreId bigint not null,
FOREIGN KEY (MovieId) 
REFERENCES Movies(Id) 
ON DELETE CASCADE 
ON UPDATE CASCADE,
FOREIGN KEY (MovieGenreId) 
REFERENCES MoviesGenres(Id)
ON DELETE CASCADE
ON UPDATE CASCADE);");
    }
}
