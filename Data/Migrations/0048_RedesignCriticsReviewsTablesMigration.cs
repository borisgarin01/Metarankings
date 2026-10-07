namespace Data.Migrations;

/// <summary>
/// Рецензия критика - это оценка издания (Канобу, IGN, ...), которую добавляет администратор,
/// а не отзыв пользователя сайта. UserId теперь хранит администратора, добавившего рецензию.
/// </summary>
[Migration(48, "Redesign critics reviews tables migration")]
public sealed class RedesignCriticsReviewsTablesMigration : Migration
{
    public override void Down()
    {
        DownFor("GamesCriticsReviews", "GameId");
        DownFor("MoviesCriticsReviews", "MovieId");
    }

    public override void Up()
    {
        UpFor("GamesCriticsReviews", "GameId");
        UpFor("MoviesCriticsReviews", "MovieId");
    }

    private void UpFor(string table, string entityColumn)
    {
        Execute.Sql($@"
ALTER TABLE {table} DROP CONSTRAINT IF EXISTS {table}_{entityColumn}_userid_key;
ALTER TABLE {table} DROP CONSTRAINT IF EXISTS {table}_userid_fkey;

ALTER TABLE {table} ALTER COLUMN UserId DROP NOT NULL;
ALTER TABLE {table} ADD FOREIGN KEY (UserId) REFERENCES ApplicationUsers(Id) ON DELETE SET NULL;

ALTER TABLE {table} ALTER COLUMN TextContent DROP NOT NULL;
ALTER TABLE {table} ALTER COLUMN Date SET DEFAULT CURRENT_DATE;

ALTER TABLE {table} ADD Publication varchar(255) null;
UPDATE {table} SET Publication = 'Издание #' || Id;
ALTER TABLE {table} ALTER COLUMN Publication SET NOT NULL;
ALTER TABLE {table} ADD Author varchar(255) null;
ALTER TABLE {table} ADD SourceUrl varchar(1023) null;

ALTER TABLE {table} ADD CONSTRAINT {table}_{entityColumn}_publication_key UNIQUE ({entityColumn}, Publication);");
    }

    private void DownFor(string table, string entityColumn)
    {
        Execute.Sql($@"
ALTER TABLE {table} DROP CONSTRAINT IF EXISTS {table}_{entityColumn}_publication_key;
ALTER TABLE {table} DROP COLUMN SourceUrl;
ALTER TABLE {table} DROP COLUMN Author;
ALTER TABLE {table} DROP COLUMN Publication;

ALTER TABLE {table} ALTER COLUMN Date DROP DEFAULT;
UPDATE {table} SET TextContent = '' WHERE TextContent IS NULL;
ALTER TABLE {table} ALTER COLUMN TextContent SET NOT NULL;

DELETE FROM {table} WHERE UserId IS NULL;
ALTER TABLE {table} DROP CONSTRAINT IF EXISTS {table}_userid_fkey;
ALTER TABLE {table} ALTER COLUMN UserId SET NOT NULL;
ALTER TABLE {table} ADD FOREIGN KEY (UserId) REFERENCES ApplicationUsers(Id) ON DELETE CASCADE;
ALTER TABLE {table} ADD UNIQUE ({entityColumn}, UserId);");
    }
}
