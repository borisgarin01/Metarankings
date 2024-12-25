using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class GameGameGenres : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Critics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(511)", maxLength: 511, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Critics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gamers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gamers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamesDevelopers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesDevelopers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamesGenres",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesGenres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamesLocalizations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesLocalizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamesPlatforms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesPlatforms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamesPublishers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesPublishers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GamesTags",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesTags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CollectionItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Href = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageSrc = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CollectionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CollectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CollectionItems_Collections_CollectionId",
                        column: x => x.CollectionId,
                        principalTable: "Collections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    ImageSource = table.Column<string>(type: "nvarchar(511)", maxLength: 511, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalizationId = table.Column<long>(type: "bigint", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_GamesLocalizations_LocalizationId",
                        column: x => x.LocalizationId,
                        principalTable: "GamesLocalizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateTable(
                name: "GameGamerReview",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GamerId = table.Column<long>(type: "bigint", nullable: false),
                    GameId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGamerReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameGamerReview_Gamers_GamerId",
                        column: x => x.GamerId,
                        principalTable: "Gamers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGamerReview_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameGameTag",
                columns: table => new
                {
                    GamesId = table.Column<long>(type: "bigint", nullable: false),
                    TagsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameGameTag", x => new { x.GamesId, x.TagsId });
                    table.ForeignKey(
                        name: "FK_GameGameTag_GamesTags_TagsId",
                        column: x => x.TagsId,
                        principalTable: "GamesTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameGameTag_Games_GamesId",
                        column: x => x.GamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesCriticsReviews",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CriticId = table.Column<long>(type: "bigint", nullable: false),
                    GameId = table.Column<long>(type: "bigint", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Score = table.Column<float>(type: "real", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(511)", maxLength: 511, nullable: false),
                    ReviewDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesCriticsReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesCriticsReviews_Critics_CriticId",
                        column: x => x.CriticId,
                        principalTable: "Critics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesCriticsReviews_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GamesGamesGenres",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<long>(type: "bigint", nullable: false),
                    GameGenreId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GamesGamesGenres", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GamesGamesGenres_GamesGenres_GameGenreId",
                        column: x => x.GameGenreId,
                        principalTable: "GamesGenres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GamesGamesGenres_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Trailers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GameId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trailers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trailers_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CollectionItems_CollectionId",
                table: "CollectionItems",
                column: "CollectionId");

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

            migrationBuilder.CreateIndex(
                name: "IX_Collections_Name",
                table: "Collections",
                column: "Name",
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

            migrationBuilder.CreateIndex(
                name: "IX_GameGameDeveloper_GamesId",
                table: "GameGameDeveloper",
                column: "GamesId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGameGenre_GenresId",
                table: "GameGameGenre",
                column: "GenresId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGamePlatform_PlatformsId",
                table: "GameGamePlatform",
                column: "PlatformsId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGamePublisher_PublishersId",
                table: "GameGamePublisher",
                column: "PublishersId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGamerReview_GameId",
                table: "GameGamerReview",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameGamerReview_GamerId",
                table: "GameGamerReview",
                column: "GamerId");

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
                name: "IX_GameGameTag_TagsId",
                table: "GameGameTag",
                column: "TagsId");

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
                name: "IX_Games_LocalizationId",
                table: "Games",
                column: "LocalizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Name",
                table: "Games",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesCriticsReviews_CriticId",
                table: "GamesCriticsReviews",
                column: "CriticId");

            migrationBuilder.CreateIndex(
                name: "IX_GamesCriticsReviews_GameId",
                table: "GamesCriticsReviews",
                column: "GameId");

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
                name: "IX_GamesGamesGenres_GameGenreId",
                table: "GamesGamesGenres",
                column: "GameGenreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GamesGamesGenres_GameId",
                table: "GamesGamesGenres",
                column: "GameId",
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
                name: "IX_Trailers_GameId",
                table: "Trailers",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Trailers_Url",
                table: "Trailers",
                column: "Url",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CollectionItems");

            migrationBuilder.DropTable(
                name: "GameGameDeveloper");

            migrationBuilder.DropTable(
                name: "GameGameGenre");

            migrationBuilder.DropTable(
                name: "GameGamePlatform");

            migrationBuilder.DropTable(
                name: "GameGamePublisher");

            migrationBuilder.DropTable(
                name: "GameGamerReview");

            migrationBuilder.DropTable(
                name: "GameGameTag");

            migrationBuilder.DropTable(
                name: "GamesCriticsReviews");

            migrationBuilder.DropTable(
                name: "GamesGamesGenres");

            migrationBuilder.DropTable(
                name: "Trailers");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "GamesDevelopers");

            migrationBuilder.DropTable(
                name: "GamesPlatforms");

            migrationBuilder.DropTable(
                name: "GamesPublishers");

            migrationBuilder.DropTable(
                name: "Gamers");

            migrationBuilder.DropTable(
                name: "GamesTags");

            migrationBuilder.DropTable(
                name: "Critics");

            migrationBuilder.DropTable(
                name: "GamesGenres");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "GamesLocalizations");
        }
    }
}
