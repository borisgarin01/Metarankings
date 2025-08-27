namespace Data.Migrations;

[Migration(30, "Add security timestamp to user migration")]
public sealed class AddSecurityTimestampToUserMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("ALTER TABLE ApplicationUsers DROP COLUMN SecurityStamp;");
    }

    public override void Up()
    {
        Execute.Sql("ALTER TABLE ApplicationUsers ADD SecurityStamp nvarchar(511) null;");
    }
}
