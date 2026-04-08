namespace Data.Migrations;

[Migration(8, "Add games developers table migration")]
public sealed class CreateGamesDevelopersTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesDevelopers");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesDevelopers
(Id bigserial not null primary key,
GameId bigint not null, 
DeveloperId bigint not null,
CONSTRAINT UNIQUE_GameId_DeveloperId UNIQUE(GameId, DeveloperId),
FOREIGN KEY(GameId) REFERENCES Games(Id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY(DeveloperId) REFERENCES Developers(Id) ON UPDATE CASCADE ON DELETE CASCADE
);");
    }
}
