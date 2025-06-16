using FluentMigrator;

namespace Data.Migrations.Movies.CreateTables;

[Migration(16, "Create movies movies studios table migration")]
public sealed class CreateMoviesMoviesStudiosTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesMoviesStudios;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesMoviesStudios
(Id bigint not null primary key identity(1,1),
MovieId bigint not null,
MovieStudioId bigint not null,
FOREIGN KEY (MovieId) 
REFERENCES Movies(Id)
ON DELETE CASCADE
ON UPDATE CASCADE,
FOREIGN KEY (MovieStudioId) 
REFERENCES MoviesStudios(Id)
ON DELETE CASCADE
ON UPDATE CASCADE);");
    }
}
