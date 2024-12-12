using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class UniqueConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "GamesCriticsReviews",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Games",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "GameGamerReview",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "GameGamerReview",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_GamesTags_Title",
                table: "GamesTags",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesTags_Url",
                table: "GamesTags",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesPublishers_Name",
                table: "GamesPublishers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesPublishers_Url",
                table: "GamesPublishers",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesPlatforms_Name",
                table: "GamesPlatforms",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesPlatforms_Url",
                table: "GamesPlatforms",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesLocalizations_Title",
                table: "GamesLocalizations",
                column: "Title",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesLocalizations_Url",
                table: "GamesLocalizations",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesGenres_Name",
                table: "GamesGenres",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesGenres_Url",
                table: "GamesGenres",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesDevelopers_Name",
                table: "GamesDevelopers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesDevelopers_Url",
                table: "GamesDevelopers",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesCriticsReviews_Text",
                table: "GamesCriticsReviews",
                column: "Text",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesCriticsReviews_Url",
                table: "GamesCriticsReviews",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_Description",
                table: "Games",
                column: "Description",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Games_Name",
                table: "Games",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gamers_AccountName",
                table: "Gamers",
                column: "AccountName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gamers_Url",
                table: "Gamers",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameGamerReview_Text",
                table: "GameGamerReview",
                column: "Text",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameGamerReview_Url",
                table: "GameGamerReview",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Critics_Name",
                table: "Critics",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Critics_Url",
                table: "Critics",
                column: "Url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GamesTags_Title",
                table: "GamesTags");

            migrationBuilder.DropIndex(
                name: "IX_GamesTags_Url",
                table: "GamesTags");

            migrationBuilder.DropIndex(
                name: "IX_GamesPublishers_Name",
                table: "GamesPublishers");

            migrationBuilder.DropIndex(
                name: "IX_GamesPublishers_Url",
                table: "GamesPublishers");

            migrationBuilder.DropIndex(
                name: "IX_GamesPlatforms_Name",
                table: "GamesPlatforms");

            migrationBuilder.DropIndex(
                name: "IX_GamesPlatforms_Url",
                table: "GamesPlatforms");

            migrationBuilder.DropIndex(
                name: "IX_GamesLocalizations_Title",
                table: "GamesLocalizations");

            migrationBuilder.DropIndex(
                name: "IX_GamesLocalizations_Url",
                table: "GamesLocalizations");

            migrationBuilder.DropIndex(
                name: "IX_GamesGenres_Name",
                table: "GamesGenres");

            migrationBuilder.DropIndex(
                name: "IX_GamesGenres_Url",
                table: "GamesGenres");

            migrationBuilder.DropIndex(
                name: "IX_GamesDevelopers_Name",
                table: "GamesDevelopers");

            migrationBuilder.DropIndex(
                name: "IX_GamesDevelopers_Url",
                table: "GamesDevelopers");

            migrationBuilder.DropIndex(
                name: "IX_GamesCriticsReviews_Text",
                table: "GamesCriticsReviews");

            migrationBuilder.DropIndex(
                name: "IX_GamesCriticsReviews_Url",
                table: "GamesCriticsReviews");

            migrationBuilder.DropIndex(
                name: "IX_Games_Description",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Games_Name",
                table: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Gamers_AccountName",
                table: "Gamers");

            migrationBuilder.DropIndex(
                name: "IX_Gamers_Url",
                table: "Gamers");

            migrationBuilder.DropIndex(
                name: "IX_GameGamerReview_Text",
                table: "GameGamerReview");

            migrationBuilder.DropIndex(
                name: "IX_GameGamerReview_Url",
                table: "GameGamerReview");

            migrationBuilder.DropIndex(
                name: "IX_Critics_Name",
                table: "Critics");

            migrationBuilder.DropIndex(
                name: "IX_Critics_Url",
                table: "Critics");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "GamesCriticsReviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Games",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "GameGamerReview",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Text",
                table: "GameGamerReview",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
