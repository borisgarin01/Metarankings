using FluentMigrator;

namespace Data.Migrations.Movies.InsertData;

[Migration(22, "Insert genres migration")]
public sealed class InsertGenresMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DELETE FROM MoviesGenres
WHERE Name in ('Боевик', 'Военный', 'Комедия', 'Ужас', 'Фантастика',
'Триллер', 'Фэнтези', 'Исторический', 'Мелодрама', 'Драма', 
'Семейный', 'Новогодний', 'Мультфильм', 'Сериал');");
    }

    public override void Up()
    {
        Execute.Sql(@"INSERT INTO MoviesGenres (Name, Href) VALUES
('Боевик', '/moviesGenres/action-movies'),
('Военный', '/moviesGenres/war'),
('Комедия', '/moviesGenres/comedies'),
('Ужас', '/moviesGenres/horror'),
('Фантастика', '/moviesGenres/fiction'),
('Триллер', '/moviesGenres/thriller'),
('Фэнтези', '/moviesGenres/fantasy'),
('Исторический', '/moviesGenres/historical'),
('Мелодрама', '/moviesGenres/melodrama'),
('Драма', '/moviesGenres/dramas'),
('Семейный', '/moviesGenres/family'),
('Новогодний', '/moviesGenres/new-year'),
('Мультфильм', '/moviesGenres/cartoon'),
('Сериал', '/moviesGenres/series');");
    }
}
