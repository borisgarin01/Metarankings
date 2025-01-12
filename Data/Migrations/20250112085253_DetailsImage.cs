using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class DetailsImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageSource",
                table: "Games",
                newName: "ListImageSource");

            migrationBuilder.AddColumn<string>(
                name: "DetailsImageSource",
                table: "Games",
                type: "nvarchar(511)",
                maxLength: 511,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DetailsImageSource",
                table: "Games");

            migrationBuilder.RenameColumn(
                name: "ListImageSource",
                table: "Games",
                newName: "ImageSource");
        }
    }
}
