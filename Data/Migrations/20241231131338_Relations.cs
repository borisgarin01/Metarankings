using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class Relations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameGameDeveloper");

            migrationBuilder.DropTable(
                name: "GameGameGenre");

            migrationBuilder.DropTable(
                name: "GameGamePublisher");

            migrationBuilder.AddForeignKey(
                name: "FK_GameGamePublishers_Games_GameId",
                table: "GameGamePublishers",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameGamePublishers_Games_GameId",
                table: "GameGamePublishers");

            migrationBuilder.CreateTable(
                name: "GameGameDeveloper",
                columns: table => new
                {
                    DevelopersId = table.Column<long>(type: "bigint", nullable: false),
                    GamesId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGameDeveloper", x => new { x.DevelopersId, x.GamesId });
                    table.ForeignKey(
                        name: "FK_GameGameDeveloper_GamesDevelopers_DevelopersId",
                        column: x => x.DevelopersId,
                        principalTable: "GamesDevelopers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGameDeveloper_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameGameGenre",
                columns: table => new
                {
                    GamesId = table.Column<long>(type: "bigint", nullable: false),
                    GenresId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGameGenre", x => new { x.GamesId, x.GenresId });
                    table.ForeignKey(
                        name: "FK_GameGameGenre_GamesGenres_GenresId",
                        column: x => x.GenresId,
                        principalTable: "GamesGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGameGenre_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameGamePublisher",
                columns: table => new
                {
                    GamesId = table.Column<long>(type: "bigint", nullable: false),
                    PublishersId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGamePublisher", x => new { x.GamesId, x.PublishersId });
                    table.ForeignKey(
                        name: "FK_GameGamePublisher_GamesPublishers_PublishersId",
                        column: x => x.PublishersId,
                        principalTable: "GamesPublishers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGamePublisher_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameGameDeveloper_GamesId",
                table: "GameGameDeveloper",
                column: "GamesId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGameGenre_GenresId",
                table: "GameGameGenre",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGamePublisher_PublishersId",
                table: "GameGamePublisher",
                column: "PublishersId");
        }
    }
}
