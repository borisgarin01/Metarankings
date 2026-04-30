namespace Data.Migrations;

[Migration(40, "Create movies collections table migration")]
public sealed class CreateMoviesCollectionsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE MoviesCollections;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesCollections(Id BIGSERIAL NOT NULL PRIMARY KEY, 
Name VARCHAR(255) NOT NULL UNIQUE,
Description TEXT NOT NULL,
ImageSource TEXT NOT NULL);");
    }
}
