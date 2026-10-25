namespace Data.Migrations;

[Migration(52, "Add games platforms order column migration")]
public sealed class AddGamesDevelopersOrderColumnMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE GamesDevelopers DROP COLUMN Position");
    }

    public override void Up()
    {
        Execute.Sql(@"ALTER TABLE GamesDevelopers ADD Position SMALLINT NULL;");
    }
}
