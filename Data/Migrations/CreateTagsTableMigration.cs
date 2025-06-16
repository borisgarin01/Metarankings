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
Id bigint not null primary key identity(1,1),
Title varchar(255) not null unique
);");
    }
}
