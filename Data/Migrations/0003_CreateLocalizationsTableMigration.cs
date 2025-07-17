namespace Data.Migrations;

[Migration(3, "Add localizations table migration")]
public sealed class CreateLocalizationsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE Localizations;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Localizations 
(Id bigint not null primary key identity(1,1),
Name nvarchar(127) not null unique);");
    }
}
