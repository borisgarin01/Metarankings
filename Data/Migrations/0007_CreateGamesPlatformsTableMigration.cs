namespace Data.Migrations;

[Migration(7, "Add games platforms table migration")]
public sealed class CreateGamesPlatformsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesPlatforms");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesPlatforms(Id bigint primary key not null identity(1,1),
GameId bigint not null, 
PlatformId bigint not null,
UNIQUE(GameId, PlatformId),
FOREIGN KEY(GameId) REFERENCES Games(Id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY(PlatformId) REFERENCES Platforms(Id) ON UPDATE CASCADE ON DELETE CASCADE
);");
    }
}
