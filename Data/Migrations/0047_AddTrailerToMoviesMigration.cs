namespace Data.Migrations;

[Migration(47, "Add trailer to movies migration")]
public sealed class AddTrailerToMoviesMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE Movies DROP COLUMN Trailer;");
    }

    public override void Up()
    {
        Execute.Sql("ALTER TABLE Movies ADD Trailer varchar(511) null;");
    }
}
