using FluentMigrator;

namespace Data.Migrations.Movies.CreateTables;

[Migration(14, "Create movies table migration")]
public sealed class CreateMoviesTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Movies;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Movies 
(Id bigint not null primary key identity(1,1),
Name varchar(255) not null unique,
ImageSource varchar(1023) null unique,
OriginalName varchar(255) not null unique,
PremierDate date not null,
Description varchar(1023) null);");
    }
}
