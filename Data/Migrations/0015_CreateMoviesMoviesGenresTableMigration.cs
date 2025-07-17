namespace Data.Migrations;

[Migration(15, "Create movies movies genres table migration")]
public sealed class CreateMoviesMoviesGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesMoviesGenres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesMoviesGenres
(Id bigint not null primary key identity(1,1),
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
