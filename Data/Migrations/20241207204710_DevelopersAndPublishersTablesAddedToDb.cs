using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class DevelopersAndPublishersTablesAddedToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "DetailsComponentItemId",
                table: "Platforms",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DetailsComponentItemId",
                table: "Genres",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Localization",
                table: "DetailsComponentsItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Developer",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailsComponentItemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Developer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Developer_DetailsComponentsItems_DetailsComponentItemId",
                        column: x => x.DetailsComponentItemId,
                        principalTable: "DetailsComponentsItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Publisher",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Href = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailsComponentItemId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publisher", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publisher_DetailsComponentsItems_DetailsComponentItemId",
                        column: x => x.DetailsComponentItemId,
                        principalTable: "DetailsComponentsItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Platforms_DetailsComponentItemId",
                table: "Platforms",
                column: "DetailsComponentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Genres_DetailsComponentItemId",
                table: "Genres",
                column: "DetailsComponentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Developer_DetailsComponentItemId",
                table: "Developer",
                column: "DetailsComponentItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Publisher_DetailsComponentItemId",
                table: "Publisher",
                column: "DetailsComponentItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Genres_DetailsComponentsItems_DetailsComponentItemId",
                table: "Genres",
                column: "DetailsComponentItemId",
                principalTable: "DetailsComponentsItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Platforms_DetailsComponentsItems_DetailsComponentItemId",
                table: "Platforms",
                column: "DetailsComponentItemId",
                principalTable: "DetailsComponentsItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Genres_DetailsComponentsItems_DetailsComponentItemId",
                table: "Genres");

            migrationBuilder.DropForeignKey(
                name: "FK_Platforms_DetailsComponentsItems_DetailsComponentItemId",
                table: "Platforms");

            migrationBuilder.DropTable(
                name: "Developer");

            migrationBuilder.DropTable(
                name: "Publisher");

            migrationBuilder.DropIndex(
                name: "IX_Platforms_DetailsComponentItemId",
                table: "Platforms");

            migrationBuilder.DropIndex(
                name: "IX_Genres_DetailsComponentItemId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "DetailsComponentItemId",
                table: "Platforms");

            migrationBuilder.DropColumn(
                name: "DetailsComponentItemId",
                table: "Genres");

            migrationBuilder.DropColumn(
                name: "Localization",
                table: "DetailsComponentsItems");
        }
    }
}
