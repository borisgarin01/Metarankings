namespace Data.Migrations.Games.CreateTables;

[Migration(2, "Add games table migration")]
public sealed class CreateGenresTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DROP TABLE Genres;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE Genres (Id bigint not null primary key identity(1,1),
Name nvarchar(255) not null unique);");
    }
}
