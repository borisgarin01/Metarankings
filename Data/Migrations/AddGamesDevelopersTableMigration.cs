using FluentMigrator;

namespace Data.Migrations;

[Migration(7, "Add games developers table migration")]
public sealed class AddGamesDevelopersTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesDevelopers");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesDevelopers(Id bigserial primary key not null,
GameId bigint not null, 
DeveloperId bigint not null,
UNIQUE(GameId, DeveloperId),
FOREIGN KEY(GameId) REFERENCES Games(Id) ON UPDATE CASCADE ON DELETE CASCADE,
FOREIGN KEY(DeveloperId) REFERENCES Developers(Id) ON UPDATE CASCADE ON DELETE CASCADE
);");
    }
}
