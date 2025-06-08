using FluentMigrator;

namespace Data.Migrations.Games.InsertData;

[Migration(15, "Insert localizations migration")]
public sealed class InsertLocalizationsMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DELETE FROM Localizations 
WHERE Name IN
('Полностью на русском языке', 'Субтитры на русском языке', 'Без русского языка');");
    }

    public override void Up()
    {
        Execute.Sql(@"INSERT INTO Localizations 
(Href, Name) 
VALUES
('/localizations/fully-localized-to-russian', 'Полностью на русском языке'), 
('/localizations/russian-subtitles', 'Субтитры на русском языке'), 
('/localizations/without-russian-language', 'Без русского языка');");
    }
}
