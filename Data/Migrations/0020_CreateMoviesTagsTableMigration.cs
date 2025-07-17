namespace Data.Migrations;

[Migration(20, "Create movies tags table migration")]
public sealed class CreateMoviesTagsTableMigration : Migration
{
    public override void Down()
    {
        Execute.Sql("DROP TABLE MoviesTags;");
    }

    public override void Up()
    {
        Execute.Sql(@"CREATE TABLE MoviesTags
(
Id bigint not null primary key identity(1,1),
MovieId bigint not null,
TagId bigint not null,
FOREIGN KEY(MovieId) REFERENCES Movies(Id)
ON DELETE CASCADE
ON UPDATE CASCADE,
FOREIGN KEY(TagId) REFERENCES Tags(Id)
ON DELETE CASCADE
ON UPDATE CASCADE
);");
    }
}
