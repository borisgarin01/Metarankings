namespace Data.Migrations.Games.CreateTables;

[Migration(19, "Create games tags table migration")]
public sealed class CreateGamesTagsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesTags;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE GamesTags
(
Id bigint not null primary key identity(1,1),
GameId bigint not null,
TagId bigint not null,
FOREIGN KEY(GameId) REFERENCES Games(Id)
ON DELETE CASCADE
ON UPDATE CASCADE,
FOREIGN KEY(TagId) REFERENCES Tags(Id)
ON DELETE CASCADE
ON UPDATE CASCADE
);");
    }
}
