using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class StudioLinkRemovedFromDetailsComponentItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetailsComponentsItems_Studios_StudioId",
                table: "DetailsComponentsItems");

            migrationBuilder.DropIndex(
                name: "IX_DetailsComponentsItems_StudioId",
                table: "DetailsComponentsItems");

            migrationBuilder.DropColumn(
                name: "StudioId",
                table: "DetailsComponentsItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StudioId",
                table: "DetailsComponentsItems",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_DetailsComponentsItems_StudioId",
                table: "DetailsComponentsItems",
                column: "StudioId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetailsComponentsItems_Studios_StudioId",
                table: "DetailsComponentsItems",
                column: "StudioId",
                principalTable: "Studios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
