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
        Execute.Sql(@"CREATE TABLE GamesDevelopers(Id bigint primary key not null identity(1,1),
GameId bigint not null, 
DeveloperId bigint not null,
UNIQUE(GameId, DeveloperId),
FOREIGN KEY(GameId) REFERENCES Games(Id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY(DeveloperId) REFERENCES Developers(Id) ON UPDATE CASCADE ON DELETE CASCADE
);");
    }
}
