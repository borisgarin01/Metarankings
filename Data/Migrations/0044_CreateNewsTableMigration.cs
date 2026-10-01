namespace Data.Migrations;

[Migration(44, "Create news table migration")]
public sealed class CreateNewsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE News;");
    }

    public override void Up()
    {
        Execute.Sql(@"
CREATE TABLE News
(Id bigserial not null primary key,
Title varchar(511) not null unique,
UserId bigint not null,
TextContent text not null,
ImageSource text not null,
PublishTimestamp timestamp not null,
FOREIGN KEY(UserId)
    REFERENCES ApplicationUsers(Id)
    ON DELETE CASCADE);");
    }
}
