using FluentMigrator;

namespace Data.Migrations;

[Migration(14, "Insert publishers migration")]
public sealed class InsertPublishersMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DELETE FROM publishers
WHERE
Name in 
('Sony Computer Entertainment', 
'Namco Bandai Games', 
'Rockstar Games',
'Kepler Interactive',
'DONT NOD',
'Larian Studios',
'Nintendo',
'Capcom',
'CD Projekt RED',
'Bandai Namco Entertainment',
'ZAUM',
'Sony Interactive Entertainment',
'Valve',
'Microsoft Game Studios',
'Deep Silver',
'Microsoft Studios',
'Square Enix',
'2K Games',
'Bandai Namco',
'Atlus',
'Electronic Arts',
'PlayStation PC LLC',
'Activision');");
    }

    public override void Up()
    {
        Execute.Sql(@"INSERT INTO publishers
(name, url)
VALUES 
('Sony Computer Entertainment', '/publishers/sony-computer-entertainment'),
('Namco Bandai Games', '/publishers/namco-bandai-games'),
('Rockstar Games', '/publishers/rockstar-games'),
('Kepler Interactive', '/publishers/kepler-interactive'),
('DONT NOD', '/publishers/dont-nod'),
('Larian Studios', '/publishers/larian-studios'),
('Nintendo', '/publishers/nintendo'),
('Capcom', '/publishers/capcom'),
('CD Projekt RED', '/publishers/cd-projekt RED'),
('Bandai Namco Entertainment', '/publishers/bandai-namco-entertainment'),
('ZAUM', '/publishers/zaum'),
('Sony Interactive Entertainment', '/publishers/sony-interactive-entertainment'),
('Valve', '/publishers/valve'),
('Microsoft Game Studios', '/publishers/microsoft-game-studios'),
('Deep Silver', '/publishers/deep-silver'),
('Microsoft Studios', '/publishers/microsoft-studios'),
('Square Enix', '/publishers/square-enix'),
('2K Games', '/publishers/2k-games'),
('Bandai Namco', '/publishers/bandai-namco'),
('Atlus', '/publishers/atlus'),
('Electronic Arts', '/publishers/electronic-arts'),
('PlayStation PC LLC', '/publishers/playstation-pc-llc'),
('Activision', '/publishers/activision');");
    }
}
