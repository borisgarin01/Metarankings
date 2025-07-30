namespace Data.Migrations;

[Migration(26, "Create games comments table")]
public sealed class CreateGamesCommentsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE GamesComments;");
    }

    public override void Up()
    {
        Execute.Sql(@"
CREATE TABLE GamesComments
(GameId bigint not null,
UserId bigint not null,
TextContent text not null,
FOREIGN KEY(GameId) 
    REFERENCES Games(Id)
    ON DELETE CASCADE,
FOREIGN KEY(UserId)
    REFERENCES ApplicationUsers(Id)
    ON DELETE CASCADE);");
    }
}
