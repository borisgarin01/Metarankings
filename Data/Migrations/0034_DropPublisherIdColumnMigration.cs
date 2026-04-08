namespace Data.Migrations;

[Migration(34, "Drop publisher id column")]
public sealed class DropPublisherIdColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"ALTER TABLE Games ADD PublisherId not null;
ALTER TABLE Games
ADD CONSTRAINT FK_Publishers_PublisherId
FOREIGN KEY (PublisherId)
REFERENCES Publishers (Id) 
ON DELETE CASCADE 
ON UPDATE CASCADE;");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE Games DROP COLUMN PublisherId;");
    }
}
