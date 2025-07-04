﻿using FluentMigrator;

namespace Data.Migrations.Games.CreateTables;

[Migration(6, "Add games table migration")]
public sealed class CreateGamesTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE Games;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Games
(Id bigserial not null primary key,
Name varchar(1023) not null unique,
Image varchar(1023) null unique,
LocalizationId bigint null,
PublisherId bigint not null,
ReleaseDate date null,
Description text,
Trailer varchar(511) null,
FOREIGN KEY(LocalizationId) REFERENCES Localizations(Id) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY(PublisherId) REFERENCES Publishers(Id) ON DELETE CASCADE ON UPDATE CASCADE);");
    }
}
