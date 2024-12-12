using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class ConstraintsViaDataAnnotations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Collections_Name",
                table: "Collections",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_Href",
                table: "CollectionItems",
                column: "Href",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_ImageSrc",
                table: "CollectionItems",
                column: "ImageSrc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_Title",
                table: "CollectionItems",
                column: "Title",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Collections_Name",
                table: "Collections");

            migrationBuilder.DropIndex(
                name: "IX_CollectionItems_Href",
                table: "CollectionItems");

            migrationBuilder.DropIndex(
                name: "IX_CollectionItems_ImageSrc",
                table: "CollectionItems");

            migrationBuilder.DropIndex(
                name: "IX_CollectionItems_Title",
                table: "CollectionItems");
        }
    }
}
