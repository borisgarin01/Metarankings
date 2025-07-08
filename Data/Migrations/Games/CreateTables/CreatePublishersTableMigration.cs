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
(Id bigint not null primary key identity(1,1),
Name nvarchar(511) not null unique);");
    }
}
