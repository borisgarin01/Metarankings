﻿namespace Data.Migrations;

[Migration(17, "Create movies movies directors table migration")]
public sealed class CreateMoviesMoviesDirectorsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesMoviesDirectors;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesMoviesDirectors
(Id bigint not null primary key identity(1,1),
MovieId bigint not null,
MovieDirectorId bigint not null,
FOREIGN KEY (MovieId) 
REFERENCES Movies(Id) 
ON DELETE CASCADE 
ON UPDATE CASCADE,
FOREIGN KEY (MovieDirectorId) 
REFERENCES MoviesDirectors(Id)
ON DELETE CASCADE
ON UPDATE CASCADE);");
    }
}
