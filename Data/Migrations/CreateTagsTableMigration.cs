using FluentMigrator;

namespace Data.Migrations;

[Migration(18, "Create tags table migration")]
public sealed class CreateTagsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Tags;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Tags
(
Id bigserial not null primary key, 
Title varchar(255) not null unique
);");
    }
}
