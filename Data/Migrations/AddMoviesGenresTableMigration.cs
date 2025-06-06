using FluentMigrator;

namespace Data.Migrations;

[Migration(17, "Add movies genres table migration")]
public sealed class AddMoviesGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesGenres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesGenres (id bigserial not null primary key,
name varchar(511) not null unique,
href varchar(1023) not null unique);");
    }
}
