﻿using FluentMigrator;

namespace Data.Migrations.Games.CreateTables;

[Migration(2, "Add games table migration")]
public sealed class CreateGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE Genres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Genres (Id bigserial not null primary key,
Name varchar(255) not null unique);");
    }
}
