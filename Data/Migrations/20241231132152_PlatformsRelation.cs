using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class PlatformsRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameGamePlatforms_Platform_PlatformId",
                table: "GameGamePlatforms");

            migrationBuilder.DropTable(
                name: "GameGamePlatform");

            migrationBuilder.DropTable(
                name: "Platform");

            migrationBuilder.RenameColumn(
                name: "PlatformId",
                table: "GameGamePlatforms",
                newName: "GamePlatformId");

            migrationBuilder.RenameIndex(
                name: "IX_GameGamePlatforms_PlatformId",
                table: "GameGamePlatforms",
                newName: "IX_GameGamePlatforms_GamePlatformId");

            migrationBuilder.RenameIndex(
                name: "IX_GameGamePlatforms_GameId_PlatformId",
                table: "GameGamePlatforms",
                newName: "IX_GameGamePlatforms_GameId_GamePlatformId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameGamePlatforms_GamesPlatforms_GamePlatformId",
                table: "GameGamePlatforms",
                column: "GamePlatformId",
                principalTable: "GamesPlatforms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameGamePlatforms_GamesPlatforms_GamePlatformId",
                table: "GameGamePlatforms");

            migrationBuilder.RenameColumn(
                name: "GamePlatformId",
                table: "GameGamePlatforms",
                newName: "PlatformId");

            migrationBuilder.RenameIndex(
                name: "IX_GameGamePlatforms_GamePlatformId",
                table: "GameGamePlatforms",
                newName: "IX_GameGamePlatforms_PlatformId");

            migrationBuilder.RenameIndex(
                name: "IX_GameGamePlatforms_GameId_GamePlatformId",
                table: "GameGamePlatforms",
                newName: "IX_GameGamePlatforms_GameId_PlatformId");

            migrationBuilder.CreateTable(
                name: "GameGamePlatform",
                columns: table => new
                {
                    GamesId = table.Column<long>(type: "bigint", nullable: false),
                    PlatformsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGamePlatform", x => new { x.GamesId, x.PlatformsId });
                    table.ForeignKey(
                        name: "FK_GameGamePlatform_GamesPlatforms_PlatformsId",
                        column: x => x.PlatformsId,
                        principalTable: "GamesPlatforms",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGamePlatform_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Platform",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Link = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Platform", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameGamePlatform_PlatformsId",
                table: "GameGamePlatform",
                column: "PlatformsId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameGamePlatforms_Platform_PlatformId",
                table: "GameGamePlatforms",
                column: "PlatformId",
                principalTable: "Platform",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
