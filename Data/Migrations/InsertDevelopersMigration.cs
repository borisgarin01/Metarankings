using FluentMigrator;

namespace Data.Migrations;

[Migration(11, "Insert developers migration")]
public sealed class InsertDevelopersMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"delete from developers where name in
(
'Naughty Dog', 
'Sony Santa Monica', 
'CD Projekt RED', 
'Rockstar Games', 
'Sandfall Interactive', 
'Studio Tolima',
'Larian Studios',
'Nintendo',
'Capcom',
'Nintendo EAD',
'FromSoftware',
'ZAUM',
'Bluepoint Games',
'Insomniac Games',
'Valve',
'Guerrilla Games',
'Epic Games',
'Warhorse Studios',
'Moon Studios',
'4A Games',
'Monster Games',
'PlatinumGames',
'Playground Games',
'Crystal Dynamix',
'Irrational Games',
'Quantic Dream',
'Team Asobi',
'Atlus Co.',
'Hazelight Studios'
)");
    }

    public override void Up()
    {
        Execute.Sql(@"insert into developers(name, url)
VALUES
('Naughty Dog', '/developers/naughty-dog'),
('Sony Santa Monica', '/developers/sony-santa-monica'),
('CD Projekt RED', '/developers/cd-project-red'),
('Rockstar Games', '/developers/rockstar-games'),
('Sandfall Interactive', '/developers/sandfall-interactive'),
('Studio Tolima', '/developers/studio-tolima'),
('Larian Studios', '/developers/larian-studios'),
('Nintendo', '/developers/nintendo'),
('Capcom', '/developers/capcom'),
('Nintendo EAD', '/developers/nintendo-ead'),
('FromSoftware', '/developers/fromsoftware'),
('ZAUM', '/developers/zaum'),
('Bluepoint Games', '/developers/bluepoint-games'),
('Insomniac Games', '/developers/insomniac-games'),
('Valve', '/developers/valve'),
('Guerrilla Games', '/developers/guerrilla-games'),
('Epic Games', '/developers/epic-games'),
('Warhorse Studios', '/developers/warhorse-studios'),
('Moon Studios', '/developers/moon-studios'),
('4A Games', '/developers/4a-games'),
('Monster Games', '/developers/monster-games'),
('PlatinumGames', '/developers/platinumgames'),
('Playground Games', '/developers/playground-games'),
('Crystal Dynamix', '/developers/crystal-dynamix'),
('Irrational Games', '/developers/irrational-games'),
('Quantic Dream', '/developers/quantic-dream'),
('Team Asobi', '/developers/team-asobi'),
('Atlus Co.', '/developers/atlus-co'),
('Hazelight Studios', '/developers/hazelight-studios');");
    }
}
