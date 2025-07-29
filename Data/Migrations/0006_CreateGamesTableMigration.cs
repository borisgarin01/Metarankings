namespace Data.Migrations;

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
(Id bigint not null primary key identity(1,1),
Name nvarchar(1023) not null unique,
Image nvarchar(1023) null unique,
LocalizationId bigint null,
PublisherId bigint not null,
ReleaseDate date null,
Description nvarchar(max),
Trailer nvarchar(511) null,
FOREIGN KEY(LocalizationId) REFERENCES Localizations(Id) ON DELETE CASCADE ON UPDATE CASCADE,
FOREIGN KEY(PublisherId) REFERENCES Publishers(Id) ON DELETE CASCADE ON UPDATE CASCADE);");
    }
}
