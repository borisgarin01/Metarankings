using FluentMigrator;

namespace Data.Migrations.Movies.CreateTables;

[Migration(12, "Create movies genres table migration")]
public sealed class CreateMoviesStudiosTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesStudios;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesStudios (Id bigint not null primary key identity(1,1),
name nvarchar(511) not null unique);");
    }
}
