namespace Data.Migrations;

[Migration(46, "Create content requests table migration")]
public sealed class CreateContentRequestsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE ContentRequests;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE ContentRequests
(
Id BIGSERIAL NOT NULL PRIMARY KEY,
UserId BIGINT NOT NULL,
ContentType SMALLINT NOT NULL, -- 1 - игра, 2 - фильм
Title VARCHAR(511) NOT NULL,
Description VARCHAR(4000) NULL,
Status SMALLINT NOT NULL DEFAULT 0, -- 0 - новая, 1 - добавлено, 2 - отклонено
AdminComment VARCHAR(2000) NULL,
CreatedTimestamp TIMESTAMPTZ NOT NULL DEFAULT NOW(),
ProcessedTimestamp TIMESTAMPTZ NULL,
CHECK (ContentType IN (1, 2)),
CHECK (Status IN (0, 1, 2)),
FOREIGN KEY(UserId)
REFERENCES ApplicationUsers(Id)
ON DELETE CASCADE);");

        Execute.Sql("CREATE INDEX IX_ContentRequests_UserId ON ContentRequests(UserId);");
        Execute.Sql("CREATE INDEX IX_ContentRequests_Status ON ContentRequests(Status);");
    }
}
