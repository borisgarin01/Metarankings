namespace Data.Migrations;

[Migration(13, "Create movies directors table migration")]
public sealed class CreateMoviesDirectorsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesDirectors");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesDirectors (Id bigserial not null primary key,
name varchar(511) not null unique);");
    }
}
