namespace Data.Migrations
{
    [Migration(26, "Create developers creation timestamps table migration")]
    public sealed class CreateDevelopersCreationTimestampsTableMigration : Migration
    {

        public override void Up()
        {
            Execute.Sql(@"CREATE TABLE DevelopersCreationTimestamps
(DeveloperId bigint not null unique, 
Timestamp DATETIME NOT NULL);");
        }
        public override void Down()
        {
            Execute.Sql("DROP TABLE DevelopersCreationTimestamps;");
        }
    }
}
