using FluentMigrator;

namespace Data.Migrations.Games.CreateTables;

[Migration(5, "Add publishers table migration")]
public sealed class CreatePublishersTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Publishers;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Publishers
(Id bigserial not null primary key,
Name varchar(511) not null unique);");
    }
}
