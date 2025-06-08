using FluentMigrator;

namespace Data.Migrations.Movies.CreateTables;

[Migration(18, "Create movies genres table migration")]
public sealed class CreateMoviesStudiosTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesStudios;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesStudios (id bigserial not null primary key,
name varchar(511) not null unique,
href varchar(1023) not null unique);");
    }
}
