namespace Data.Migrations;

/// <summary>
/// Страны производства фильмов и съёмочная группа (актёры, продюсеры, операторы, сценаристы,
/// композиторы, художники, монтажёры). Режиссёры остаются в отдельной таблице MoviesDirectors.
/// Персона одна на все роли: один человек может быть и продюсером, и сценаристом.
/// Роль хранится числом (Domain.Movies.MovieCrewRole), Position задаёт порядок внутри роли.
/// </summary>
[Migration(50, "Create movies countries and crew tables migration")]
public sealed class CreateMoviesCountriesAndCrewTablesMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE MoviesMoviesPersons;
DROP TABLE MoviesPersons;
DROP TABLE MoviesMoviesCountries;
DROP TABLE MoviesCountries;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesCountries
(Id bigserial not null primary key,
Name varchar(255) not null unique);");

        Execute.Sql(@"CREATE TABLE MoviesMoviesCountries
(Id bigserial not null primary key,
MovieId bigint not null,
MovieCountryId bigint not null,
UNIQUE (MovieId, MovieCountryId),
FOREIGN KEY (MovieId)
REFERENCES Movies(Id)
ON DELETE CASCADE
ON UPDATE CASCADE,
FOREIGN KEY (MovieCountryId)
REFERENCES MoviesCountries(Id)
ON DELETE CASCADE
ON UPDATE CASCADE);");

        Execute.Sql(@"CREATE TABLE MoviesPersons
(Id bigserial not null primary key,
Name varchar(511) not null unique);");

        Execute.Sql(@"CREATE TABLE MoviesMoviesPersons
(Id bigserial not null primary key,
MovieId bigint not null,
MoviePersonId bigint not null,
Role smallint not null CHECK (Role BETWEEN 1 AND 7),
Position int not null default 0,
UNIQUE (MovieId, Role, MoviePersonId),
FOREIGN KEY (MovieId)
REFERENCES Movies(Id)
ON DELETE CASCADE
ON UPDATE CASCADE,
FOREIGN KEY (MoviePersonId)
REFERENCES MoviesPersons(Id)
ON DELETE CASCADE
ON UPDATE CASCADE);");

        // MovieId покрыт ведущими колонками UNIQUE, нужны обратные индексы для фильтров по стране/персоне
        Execute.Sql("CREATE INDEX IX_MoviesMoviesCountries_MovieCountryId ON MoviesMoviesCountries(MovieCountryId, MovieId);");
        Execute.Sql("CREATE INDEX IX_MoviesMoviesPersons_MoviePersonId ON MoviesMoviesPersons(MoviePersonId, MovieId);");
    }
}
