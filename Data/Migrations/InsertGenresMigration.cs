using FluentMigrator;

namespace Data.Migrations;

[Migration(12, "Insert genres migration")]
public sealed class InsertGenresMigration : Migration
{
    public override void Down()
    {
		Execute.Sql(@"DELETE FROM Genres 
WHERE
Name IN 
('Приключение',
'Хоррор',
'Экшен',
'РПГ',
'Японское РПГ',
'Аркада',
'Платформер',
'Головоломка',
'Шутер',
'Стратегия',
'Гонки');");
    }

    public override void Up()
    {
		Execute.Sql(@"INSERT INTO genres(name, url)
	VALUES 
	('Приключение', '/genres/adventure'),
	('Хоррор', '/genres/horror'),
	('Экшен', '/genres/action'),
	('РПГ', '/genres/rpg'),
	('Японское РПГ', '/genres/jrpg'),
	('Аркада', '/genres/arcade'),
	('Платформер', '/genres/platformer'),
	('Головоломка', '/genres/puzzle'),
	('Шутер', '/genres/shooter'),
	('Стратегия', '/genres/strategy'),
	('Гонки', '/genres/racing');");
    }
}
