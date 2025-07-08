using FluentMigrator;

namespace Data.Migrations.Games.CreateTables;

[Migration(1, "Add developers table migration")]
public sealed class CreateDevelopersTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Developers;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Developers
(Id bigint not null primary key identity(1,1),
Name nvarchar(511) not null unique);");
    }
}
