using FluentMigrator;

namespace Data.Migrations;

[Migration(13, "Insert platforms migration")]
public sealed class InsertPlatformsMigration : Migration
{
    public override void Down()
    {
        Execute.Sql(@"DELETE FROM platforms
WHERE 
Name IN
('PS4','PS3','PC','Switch','Xbox One','PS5', 'Xbox Series X', 'Xbox 360', 'Wii U', '3DS');");
    }

    public override void Up()
    {
        Execute.Sql(@"INSERT INTO platforms
(name, href)
VALUES 
('PS4', '/platforms/ps4'),
('PS3', '/platforms/ps3'),
('PC', '/platforms/pc'),
('Switch', '/platforms/switch'),
('Xbox One', '/platforms/xbox-one'),
('PS5', '/platforms/ps5'),
('Xbox Series X', '/platforms/xbox-series-x'),
('Xbox 360', '/platforms/xbox-360'),
('Wii U', '/platforms/wii-u'),
('3DS', '/platforms/3ds');");
    }
}
