using FluentMigrator;

namespace Data.Migrations.Movies.CreateTables;

[Migration(13, "Create movies directors table migration")]
public sealed class CreateMoviesDirectorsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesDirectors");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesDirectors (Id bigint not null primary key identity(1,1),
name nvarchar(511) not null unique);");
    }
}
