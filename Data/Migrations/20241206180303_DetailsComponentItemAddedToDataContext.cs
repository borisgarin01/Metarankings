using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class DetailsComponentItemAddedToDataContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailsComponentsItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageSource = table.Column<string>(type: "nvarchar(511)", maxLength: 511, nullable: false),
                    CriticsAverageScore = table.Column<float>(type: "real", nullable: false),
                    GamersAverageScore = table.Column<float>(type: "real", nullable: false),
                    Metarating = table.Column<float>(type: "real", nullable: false),
                    ExpectationsPercent = table.Column<float>(type: "real", nullable: false),
                    MarksCount = table.Column<int>(type: "int", nullable: false),
                    OriginalName = table.Column<string>(type: "nvarchar(511)", maxLength: 511, nullable: false),
                    StudioId = table.Column<long>(type: "bigint", nullable: false),
                    PremiereDate = table.Column<DateOnly>(type: "date", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailsComponentsItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailsComponentsItems_Studios_StudioId",
                        column: x => x.StudioId,
                        principalTable: "Studios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetailsComponentsItems_StudioId",
                table: "DetailsComponentsItems",
                column: "StudioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailsComponentsItems");
        }
    }
}
